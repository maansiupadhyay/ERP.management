using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using CollegeERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IGenericRepository<Student> _studentRepo;
    private readonly IGenericRepository<Faculty> _facultyRepo;
    private readonly IGenericRepository<Department> _deptRepo;
    private readonly IGenericRepository<Course> _courseRepo;
    private readonly IGenericRepository<AttendanceSession> _sessionRepo;
    private readonly IGenericRepository<AttendanceRecord> _recordRepo;
    private readonly IGenericRepository<Timetable> _timetableRepo;
    private readonly IGenericRepository<Exam> _examRepo;
    private readonly IGenericRepository<CourseFaculty> _cfRepo;

    public DashboardService(
        IGenericRepository<Student> studentRepo, IGenericRepository<Faculty> facultyRepo,
        IGenericRepository<Department> deptRepo, IGenericRepository<Course> courseRepo,
        IGenericRepository<AttendanceSession> sessionRepo, IGenericRepository<AttendanceRecord> recordRepo,
        IGenericRepository<Timetable> timetableRepo, IGenericRepository<Exam> examRepo,
        IGenericRepository<CourseFaculty> cfRepo)
    {
        _studentRepo = studentRepo; _facultyRepo = facultyRepo;
        _deptRepo = deptRepo; _courseRepo = courseRepo;
        _sessionRepo = sessionRepo; _recordRepo = recordRepo;
        _timetableRepo = timetableRepo; _examRepo = examRepo;
        _cfRepo = cfRepo;
    }

    public async Task<DashboardStatsDto> GetAdminStatsAsync()
    {
        var totalStudents = await _studentRepo.CountAsync();
        var totalFaculty = await _facultyRepo.CountAsync();
        var totalDepts = await _deptRepo.CountAsync();
        var totalCourses = await _courseRepo.CountAsync();
        var todaysSessions = await _sessionRepo.CountAsync(s => s.Date.Date == DateTime.UtcNow.Date);

        var allRecords = await _recordRepo.Query().ToListAsync();
        var overallAttendance = allRecords.Count > 0
            ? Math.Round((double)allRecords.Count(r => r.IsPresent) / allRecords.Count * 100, 1) : 0;

        var depts = await _deptRepo.Query()
            .Include(d => d.Students).Include(d => d.Faculties).ToListAsync();

        var deptStats = depts.Select(d => new DepartmentStatDto
        {
            DepartmentName = d.Name,
            StudentCount = d.Students?.Count ?? 0,
            FacultyCount = d.Faculties?.Count ?? 0,
            AttendancePercentage = 0
        }).ToList();

        var recentSessions = await _sessionRepo.Query()
            .Include(s => s.Course).Include(s => s.Faculty)
            .OrderByDescending(s => s.CreatedAt).Take(5).ToListAsync();

        var activities = recentSessions.Select(s => new RecentActivityDto
        {
            Description = $"Attendance marked for {s.Course?.Name} by {s.Faculty?.FirstName}",
            Timestamp = s.CreatedAt,
            Type = "Attendance"
        }).ToList();

        return new DashboardStatsDto
        {
            TotalStudents = totalStudents, TotalFaculty = totalFaculty,
            TotalDepartments = totalDepts, TotalCourses = totalCourses,
            OverallAttendance = overallAttendance, TodaysSessions = todaysSessions,
            DepartmentStats = deptStats, RecentActivities = activities
        };
    }

    public async Task<FacultyDashboardDto> GetFacultyDashboardAsync(int facultyId)
    {
        var today = DateTime.UtcNow.DayOfWeek;
        var todaySchedule = await _timetableRepo.Query()
            .Include(t => t.Course).Include(t => t.Faculty)
            .Where(t => t.FacultyId == facultyId && t.DayOfWeek == today)
            .OrderBy(t => t.StartTime).ToListAsync();

        var assignedCourseIds = await _cfRepo.Query()
            .Where(cf => cf.FacultyId == facultyId)
            .Select(cf => cf.CourseId).Distinct().ToListAsync();

        var courses = await _courseRepo.Query()
            .Include(c => c.Department)
            .Where(c => assignedCourseIds.Contains(c.Id)).ToListAsync();

        var studentCount = await _studentRepo.Query()
            .Where(s => courses.Select(c => c.DepartmentId).Contains(s.DepartmentId))
            .CountAsync();

        return new FacultyDashboardDto
        {
            TotalCourses = courses.Count,
            TodaysClasses = todaySchedule.Count,
            TotalStudents = studentCount,
            AverageAttendance = 0,
            TodaySchedule = todaySchedule.Select(t => new TimetableDto
            {
                Id = t.Id, CourseId = t.CourseId, CourseName = t.Course?.Name ?? "",
                CourseCode = t.Course?.Code ?? "", FacultyId = t.FacultyId,
                FacultyName = t.Faculty != null ? $"{t.Faculty.FirstName} {t.Faculty.LastName}" : "",
                DayOfWeek = t.DayOfWeek.ToString(),
                StartTime = t.StartTime.ToString(@"hh\:mm"),
                EndTime = t.EndTime.ToString(@"hh\:mm"),
                Room = t.Room, Section = t.Section
            }).ToList(),
            AssignedCourses = courses.Select(c => new CourseDto
            {
                Id = c.Id, Name = c.Name, Code = c.Code,
                DepartmentName = c.Department?.Name ?? "",
                Credits = c.Credits, Semester = c.Semester
            }).ToList()
        };
    }

    public async Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student == null) return new StudentDashboardDto();

        var today = DateTime.UtcNow.DayOfWeek;
        var schedule = await _timetableRepo.Query()
            .Include(t => t.Course).Include(t => t.Faculty)
            .Where(t => t.Course.DepartmentId == student.DepartmentId
                && t.Semester == student.Semester && t.DayOfWeek == today)
            .OrderBy(t => t.StartTime).ToListAsync();

        var records = await _recordRepo.Query()
            .Include(r => r.Session).ThenInclude(s => s.Course)
            .Where(r => r.StudentId == studentId).ToListAsync();

        var courseGroups = records.GroupBy(r => r.Session.CourseId);
        var courseAttendance = courseGroups.Select(g =>
        {
            var course = g.First().Session.Course;
            var total = g.Count(); var present = g.Count(r => r.IsPresent);
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
        var upcomingExams = await _examRepo.CountAsync(e => e.Date > DateTime.UtcNow);

        return new StudentDashboardDto
        {
            OverallAttendance = overallTotal > 0 ? Math.Round((double)overallPresent / overallTotal * 100, 1) : 0,
            TotalCourses = courseAttendance.Count,
            UpcomingExams = upcomingExams,
            CourseAttendance = courseAttendance,
            TodaySchedule = schedule.Select(t => new TimetableDto
            {
                Id = t.Id, CourseId = t.CourseId, CourseName = t.Course?.Name ?? "",
                CourseCode = t.Course?.Code ?? "", FacultyId = t.FacultyId,
                FacultyName = t.Faculty != null ? $"{t.Faculty.FirstName} {t.Faculty.LastName}" : "",
                DayOfWeek = t.DayOfWeek.ToString(),
                StartTime = t.StartTime.ToString(@"hh\:mm"),
                EndTime = t.EndTime.ToString(@"hh\:mm"),
                Room = t.Room, Section = t.Section
            }).ToList()
        };
    }
}
