using CollegeERP.Domain.Enums;

namespace CollegeERP.Domain.Entities;

public class AttendanceSession
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int FacultyId { get; set; }
    public DateTime Date { get; set; }
    public int? TimetableId { get; set; }
    public AttendanceSessionStatus Status { get; set; } = AttendanceSessionStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Course Course { get; set; } = null!;
    public Faculty Faculty { get; set; } = null!;
    public Timetable? Timetable { get; set; }
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}
