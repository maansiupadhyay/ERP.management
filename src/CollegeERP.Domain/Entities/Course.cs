namespace CollegeERP.Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int Credits { get; set; }
    public int Semester { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Department Department { get; set; } = null!;
    public ICollection<CourseFaculty> CourseFaculties { get; set; } = new List<CourseFaculty>();
    public ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
    public ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
