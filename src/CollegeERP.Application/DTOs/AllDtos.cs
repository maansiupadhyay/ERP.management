using System.ComponentModel.DataAnnotations;

namespace CollegeERP.Application.DTOs;

// ==================== AUTH ====================
public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string Role { get; set; } = "Student";
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
    public int? Semester { get; set; }
    public string? Section { get; set; }
    public string? Phone { get; set; }
    public string? EnrollmentNumber { get; set; }
    public string? EmployeeId { get; set; }
    public string? Designation { get; set; }
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string? Message { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int? ProfileId { get; set; }
}

// ==================== STUDENT ====================
public class StudentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EnrollmentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string Section { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime AdmissionDate { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class CreateStudentRequest
{
    [Required] public string EnrollmentNumber { get; set; } = string.Empty;
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = "Student@123";
    [Required] public int DepartmentId { get; set; }
    [Required] public int Semester { get; set; } = 1;
    public string Section { get; set; } = "A";
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-18);
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
}

public class UpdateStudentRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? DepartmentId { get; set; }
    public int? Semester { get; set; }
    public string? Section { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

// ==================== FACULTY ====================
public class FacultyDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class CreateFacultyRequest
{
    [Required] public string EmployeeId { get; set; } = string.Empty;
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = "Faculty@123";
    [Required] public int DepartmentId { get; set; }
    [Required] public string Designation { get; set; } = "Assistant Professor";
    public string? Specialization { get; set; }
    public string Phone { get; set; } = string.Empty;
}

public class UpdateFacultyRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? DepartmentId { get; set; }
    public string? Designation { get; set; }
    public string? Specialization { get; set; }
    public string? Phone { get; set; }
}

// ==================== DEPARTMENT ====================
public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? HeadOfDepartmentId { get; set; }
    public string? HodName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int StudentCount { get; set; }
    public int FacultyCount { get; set; }
    public int CourseCount { get; set; }
}

public class CreateDepartmentRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
    public int? HeadOfDepartmentId { get; set; }
    public string? Description { get; set; }
}

// ==================== COURSE ====================
public class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int Semester { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<string> AssignedFaculty { get; set; } = new();
}

public class CreateCourseRequest
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public int DepartmentId { get; set; }
    [Required] public int Credits { get; set; }
    [Required] public int Semester { get; set; }
    public string? Description { get; set; }
}

public class AssignFacultyRequest
{
    [Required] public int CourseId { get; set; }
    [Required] public int FacultyId { get; set; }
    [Required] public string AcademicYear { get; set; } = string.Empty;
    [Required] public int Semester { get; set; }
}

// ==================== ATTENDANCE ====================
public class AttendanceSessionDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int FacultyId { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
}

public class CreateAttendanceSessionRequest
{
    [Required] public int CourseId { get; set; }
    [Required] public int FacultyId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int? TimetableId { get; set; }
}

public class MarkAttendanceRequest
{
    [Required] public int SessionId { get; set; }
    [Required] public List<AttendanceEntry> Entries { get; set; } = new();
}

public class AttendanceEntry
{
    public int StudentId { get; set; }
    public bool IsPresent { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public bool IsPresent { get; set; }
    public DateTime MarkedAt { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceReportDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public List<StudentAttendanceDetail> StudentDetails { get; set; } = new();
}

public class StudentAttendanceDetail
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public double Percentage { get; set; }
    public bool IsLowAttendance { get; set; }
}

public class StudentAttendanceSummaryDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public double OverallPercentage { get; set; }
    public List<CourseAttendanceDto> CourseWise { get; set; } = new();
}

public class CourseAttendanceDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public double Percentage { get; set; }
    public bool IsLowAttendance { get; set; }
}

// ==================== TIMETABLE ====================
public class TimetableDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int FacultyId { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
}

public class CreateTimetableRequest
{
    [Required] public int CourseId { get; set; }
    [Required] public int FacultyId { get; set; }
    [Required] public string DayOfWeek { get; set; } = string.Empty;
    [Required] public string StartTime { get; set; } = string.Empty;
    [Required] public string EndTime { get; set; } = string.Empty;
    [Required] public string Room { get; set; } = string.Empty;
    public string Section { get; set; } = "A";
    [Required] public string AcademicYear { get; set; } = string.Empty;
    [Required] public int Semester { get; set; }
}

// ==================== EXAM ====================
public class ExamDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public int TotalMarks { get; set; }
    public DateTime Date { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int ResultCount { get; set; }
}

public class CreateExamRequest
{
    [Required] public int CourseId { get; set; }
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string ExamType { get; set; } = "Midterm";
    [Required] public int TotalMarks { get; set; } = 100;
    [Required] public DateTime Date { get; set; }
    [Required] public int Semester { get; set; }
    [Required] public string AcademicYear { get; set; } = string.Empty;
}

public class ExamResultDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentNumber { get; set; } = string.Empty;
    public double MarksObtained { get; set; }
    public int TotalMarks { get; set; }
    public string? Grade { get; set; }
    public string? Remarks { get; set; }
}

public class RecordResultRequest
{
    [Required] public int StudentId { get; set; }
    [Required] public double MarksObtained { get; set; }
    public string? Remarks { get; set; }
}

// ==================== DASHBOARD ====================
public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int TotalFaculty { get; set; }
    public int TotalDepartments { get; set; }
    public int TotalCourses { get; set; }
    public double OverallAttendance { get; set; }
    public int TodaysSessions { get; set; }
    public List<DepartmentStatDto> DepartmentStats { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class DepartmentStatDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public int FacultyCount { get; set; }
    public double AttendancePercentage { get; set; }
}

public class RecentActivityDto
{
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class FacultyDashboardDto
{
    public int TotalCourses { get; set; }
    public int TodaysClasses { get; set; }
    public int TotalStudents { get; set; }
    public double AverageAttendance { get; set; }
    public List<TimetableDto> TodaySchedule { get; set; } = new();
    public List<CourseDto> AssignedCourses { get; set; } = new();
}

public class StudentDashboardDto
{
    public double OverallAttendance { get; set; }
    public int TotalCourses { get; set; }
    public int UpcomingExams { get; set; }
    public List<CourseAttendanceDto> CourseAttendance { get; set; } = new();
    public List<TimetableDto> TodaySchedule { get; set; } = new();
}
