namespace CollegeERP.Domain.Entities;

public class Timetable
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int FacultyId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Room { get; set; } = string.Empty;
    public string Section { get; set; } = "A";
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Course Course { get; set; } = null!;
    public Faculty Faculty { get; set; } = null!;
}
