namespace CollegeERP.Domain.Entities;

public class Faculty
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public ICollection<CourseFaculty> CourseFaculties { get; set; } = new List<CourseFaculty>();
    public ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
    public ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
