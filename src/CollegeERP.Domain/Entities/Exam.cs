using CollegeERP.Domain.Enums;

namespace CollegeERP.Domain.Entities;

public class Exam
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ExamType ExamType { get; set; }
    public int TotalMarks { get; set; }
    public DateTime Date { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Course Course { get; set; } = null!;
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}
