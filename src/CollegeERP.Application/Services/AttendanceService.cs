using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using CollegeERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IGenericRepository<AttendanceSession> _sessionRepo;
    private readonly IGenericRepository<AttendanceRecord> _recordRepo;
    private readonly IGenericRepository<Student> _studentRepo;

    public AttendanceService(
        IGenericRepository<AttendanceSession> sessionRepo,
        IGenericRepository<AttendanceRecord> recordRepo,
        IGenericRepository<Student> studentRepo)
    {
        _sessionRepo = sessionRepo;
        _recordRepo = recordRepo;
        _studentRepo = studentRepo;
    }

    public async Task<AttendanceSessionDto> CreateSessionAsync(CreateAttendanceSessionRequest request)
    {
        var session = new AttendanceSession
        {
            CourseId = request.CourseId,
            FacultyId = request.FacultyId,
            Date = request.Date,
            TimetableId = request.TimetableId,
            Status = AttendanceSessionStatus.Open
        };
        await _sessionRepo.AddAsync(session);
        return (await GetSessionByIdAsync(session.Id))!;
    }

    public async Task<bool> MarkAttendanceAsync(MarkAttendanceRequest request)
    {
        var session = await _sessionRepo.GetByIdAsync(request.SessionId);
        if (session == null) return false;

        // Remove existing records for this session
        var existing = await _recordRepo.FindAsync(r => r.SessionId == request.SessionId);
        foreach (var r in existing) await _recordRepo.DeleteAsync(r);

        foreach (var entry in request.Entries)
        {
            await _recordRepo.AddAsync(new AttendanceRecord
            {
                SessionId = request.SessionId,
                StudentId = entry.StudentId,
                IsPresent = entry.IsPresent,
                Remarks = entry.Remarks
            });
        }

        session.Status = AttendanceSessionStatus.Closed;
        await _sessionRepo.UpdateAsync(session);
        return true;
    }

    public async Task<bool> CloseSessionAsync(int sessionId)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session == null) return false;
        session.Status = AttendanceSessionStatus.Closed;
        await _sessionRepo.UpdateAsync(session);
        return true;
    }

    public async Task<IEnumerable<AttendanceSessionDto>> GetSessionsByDateAsync(DateTime date, int? facultyId = null)
    {
        var query = _sessionRepo.Query()
            .Include(s => s.Course).Include(s => s.Faculty)
            .Include(s => s.AttendanceRecords)
            .Where(s => s.Date.Date == date.Date);

        if (facultyId.HasValue) query = query.Where(s => s.FacultyId == facultyId.Value);

        var sessions = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
        return sessions.Select(MapSessionToDto);
    }

    public async Task<IEnumerable<AttendanceSessionDto>> GetSessionsByCourseAsync(int courseId)
    {
        var sessions = await _sessionRepo.Query()
            .Include(s => s.Course).Include(s => s.Faculty)
            .Include(s => s.AttendanceRecords)
            .Where(s => s.CourseId == courseId)
            .OrderByDescending(s => s.Date).ToListAsync();
        return sessions.Select(MapSessionToDto);
    }

    public async Task<AttendanceReportDto> GetAttendanceReportAsync(int courseId, int? studentId = null)
    {
        var sessions = await _sessionRepo.Query()
            .Include(s => s.Course)
            .Include(s => s.AttendanceRecords).ThenInclude(r => r.Student)
            .Where(s => s.CourseId == courseId && s.Status == AttendanceSessionStatus.Closed)
            .ToListAsync();

        var course = sessions.FirstOrDefault()?.Course;
        var allRecords = sessions.SelectMany(s => s.AttendanceRecords);

        if (studentId.HasValue)
            allRecords = allRecords.Where(r => r.StudentId == studentId.Value);

        var studentGroups = allRecords.GroupBy(r => r.StudentId);
        var details = studentGroups.Select(g =>
        {
            var student = g.First().Student;
            var total = g.Count();
            var present = g.Count(r => r.IsPresent);
            var pct = total > 0 ? (double)present / total * 100 : 0;
            return new StudentAttendanceDetail
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                EnrollmentNumber = student.EnrollmentNumber,
                TotalPresent = present, TotalAbsent = total - present,
                Percentage = Math.Round(pct, 1),
                IsLowAttendance = pct < 75
            };
        }).OrderBy(d => d.StudentName).ToList();

        return new AttendanceReportDto
        {
            CourseId = courseId,
            CourseName = course?.Name ?? "",
            TotalSessions = sessions.Count,
            StudentDetails = details
        };
    }

    public async Task<StudentAttendanceSummaryDto> GetStudentAttendanceSummaryAsync(int studentId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student == null) return new StudentAttendanceSummaryDto();

        var records = await _recordRepo.Query()
            .Include(r => r.Session).ThenInclude(s => s.Course)
            .Where(r => r.StudentId == studentId).ToListAsync();

        var courseGroups = records.GroupBy(r => r.Session.CourseId);
        var courseWise = courseGroups.Select(g =>
        {
            var course = g.First().Session.Course;
            var total = g.Count();
            var present = g.Count(r => r.IsPresent);
            var pct = total > 0 ? (double)present / total * 100 : 0;
            return new CourseAttendanceDto
            {
                CourseId = course.Id, CourseName = course.Name, CourseCode = course.Code,
                TotalSessions = total, PresentCount = present,
                Percentage = Math.Round(pct, 1), IsLowAttendance = pct < 75
            };
        }).ToList();

        var overallTotal = records.Count;
        var overallPresent = records.Count(r => r.IsPresent);

        return new StudentAttendanceSummaryDto
        {
            StudentId = studentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            OverallPercentage = overallTotal > 0 ? Math.Round((double)overallPresent / overallTotal * 100, 1) : 0,
            CourseWise = courseWise
        };
    }

    public async Task<IEnumerable<AttendanceRecordDto>> GetSessionRecordsAsync(int sessionId)
    {
        var records = await _recordRepo.Query()
            .Include(r => r.Student)
            .Where(r => r.SessionId == sessionId)
            .OrderBy(r => r.Student.FirstName).ToListAsync();

        return records.Select(r => new AttendanceRecordDto
        {
            Id = r.Id, StudentId = r.StudentId,
            StudentName = $"{r.Student.FirstName} {r.Student.LastName}",
            EnrollmentNumber = r.Student.EnrollmentNumber,
            IsPresent = r.IsPresent, MarkedAt = r.MarkedAt, Remarks = r.Remarks
        });
    }

    private async Task<AttendanceSessionDto?> GetSessionByIdAsync(int id)
    {
        var s = await _sessionRepo.Query()
            .Include(s => s.Course).Include(s => s.Faculty)
            .Include(s => s.AttendanceRecords)
            .FirstOrDefaultAsync(s => s.Id == id);
        return s == null ? null : MapSessionToDto(s);
    }

    private static AttendanceSessionDto MapSessionToDto(AttendanceSession s) => new()
    {
        Id = s.Id, CourseId = s.CourseId, CourseName = s.Course?.Name ?? "",
        CourseCode = s.Course?.Code ?? "", FacultyId = s.FacultyId,
        FacultyName = s.Faculty != null ? $"{s.Faculty.FirstName} {s.Faculty.LastName}" : "",
        Date = s.Date, Status = s.Status.ToString(),
        TotalStudents = s.AttendanceRecords?.Count ?? 0,
        PresentCount = s.AttendanceRecords?.Count(r => r.IsPresent) ?? 0,
        AbsentCount = s.AttendanceRecords?.Count(r => !r.IsPresent) ?? 0
    };
}
