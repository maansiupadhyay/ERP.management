namespace CollegeERP.Domain.Entities;

public class ExamResult
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public double MarksObtained { get; set; }
    public string? Grade { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Exam Exam { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
