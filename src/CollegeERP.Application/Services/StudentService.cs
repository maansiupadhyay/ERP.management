using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using CollegeERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class StudentService : IStudentService
{
    private readonly IGenericRepository<Student> _studentRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<AttendanceRecord> _attendanceRepo;
    private readonly IGenericRepository<AttendanceSession> _sessionRepo;

    public StudentService(
        IGenericRepository<Student> studentRepo,
        IGenericRepository<User> userRepo,
        IGenericRepository<AttendanceRecord> attendanceRepo,
        IGenericRepository<AttendanceSession> sessionRepo)
    {
        _studentRepo = studentRepo;
        _userRepo = userRepo;
        _attendanceRepo = attendanceRepo;
        _sessionRepo = sessionRepo;
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        var students = await _studentRepo.Query()
            .Include(s => s.Department)
            .Include(s => s.User)
            .OrderBy(s => s.FirstName)
            .ToListAsync();

        return students.Select(MapToDto);
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        var student = await _studentRepo.Query()
            .Include(s => s.Department)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        return student == null ? null : MapToDto(student);
    }

    public async Task<IEnumerable<StudentDto>> GetByDepartmentAsync(int departmentId)
    {
        var students = await _studentRepo.Query()
            .Include(s => s.Department)
            .Include(s => s.User)
            .Where(s => s.DepartmentId == departmentId)
            .OrderBy(s => s.FirstName)
            .ToListAsync();

        return students.Select(MapToDto);
    }

    public async Task<IEnumerable<StudentDto>> GetBySemesterAsync(int departmentId, int semester)
    {
        var students = await _studentRepo.Query()
            .Include(s => s.Department)
            .Include(s => s.User)
            .Where(s => s.DepartmentId == departmentId && s.Semester == semester)
            .OrderBy(s => s.FirstName)
            .ToListAsync();

        return students.Select(MapToDto);
    }

    public async Task<StudentDto> CreateAsync(CreateStudentRequest request)
    {
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Student,
            IsActive = true
        };
        await _userRepo.AddAsync(user);

        var student = new Student
        {
            UserId = user.Id,
            EnrollmentNumber = request.EnrollmentNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DepartmentId = request.DepartmentId,
            Semester = request.Semester,
            Section = request.Section,
            DateOfBirth = request.DateOfBirth,
            Phone = request.Phone,
            Address = request.Address
        };
        await _studentRepo.AddAsync(student);

        return (await GetByIdAsync(student.Id))!;
    }

    public async Task<StudentDto?> UpdateAsync(int id, UpdateStudentRequest request)
    {
        var student = await _studentRepo.Query()
            .Include(s => s.Department)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) return null;

        if (request.FirstName != null) student.FirstName = request.FirstName;
        if (request.LastName != null) student.LastName = request.LastName;
        if (request.DepartmentId.HasValue) student.DepartmentId = request.DepartmentId.Value;
        if (request.Semester.HasValue) student.Semester = request.Semester.Value;
        if (request.Section != null) student.Section = request.Section;
        if (request.Phone != null) student.Phone = request.Phone;
        if (request.Address != null) student.Address = request.Address;
        student.UpdatedAt = DateTime.UtcNow;

        await _studentRepo.UpdateAsync(student);
        return MapToDto(student);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _studentRepo.GetByIdAsync(id);
        if (student == null) return false;

        var user = await _userRepo.GetByIdAsync(student.UserId);
        if (user != null)
        {
            user.IsActive = false;
            await _userRepo.UpdateAsync(user);
        }
        await _studentRepo.DeleteAsync(student);
        return true;
    }

    public async Task<StudentAttendanceSummaryDto> GetAttendanceSummaryAsync(int studentId)
    {
        var student = await _studentRepo.Query()
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            return new StudentAttendanceSummaryDto();

        var records = await _attendanceRepo.Query()
            .Include(r => r.Session)
                .ThenInclude(s => s.Course)
            .Where(r => r.StudentId == studentId)
            .ToListAsync();

        var courseGroups = records.GroupBy(r => r.Session.CourseId);
        var courseWise = courseGroups.Select(g =>
        {
            var course = g.First().Session.Course;
            var total = g.Count();
            var present = g.Count(r => r.IsPresent);
            var pct = total > 0 ? (double)present / total * 100 : 0;
            return new CourseAttendanceDto
            {
                CourseId = course.Id,
                CourseName = course.Name,
                CourseCode = course.Code,
                TotalSessions = total,
                PresentCount = present,
                Percentage = Math.Round(pct, 1),
                IsLowAttendance = pct < 75
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

    private static StudentDto MapToDto(Student s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        EnrollmentNumber = s.EnrollmentNumber,
        FirstName = s.FirstName,
        LastName = s.LastName,
        DepartmentId = s.DepartmentId,
        DepartmentName = s.Department?.Name ?? "",
        Semester = s.Semester,
        Section = s.Section,
        DateOfBirth = s.DateOfBirth,
        Phone = s.Phone,
        Address = s.Address,
        AdmissionDate = s.AdmissionDate,
        Email = s.User?.Email ?? ""
    };
}
