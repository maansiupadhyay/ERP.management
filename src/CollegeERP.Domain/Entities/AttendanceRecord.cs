namespace CollegeERP.Domain.Entities;

public class AttendanceRecord
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int StudentId { get; set; }
    public bool IsPresent { get; set; }
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }

    // Navigation
    public AttendanceSession Session { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
