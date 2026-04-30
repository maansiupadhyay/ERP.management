namespace CollegeERP.Domain.Entities;

public class CourseFaculty
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int FacultyId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Course Course { get; set; } = null!;
    public Faculty Faculty { get; set; } = null!;
}
