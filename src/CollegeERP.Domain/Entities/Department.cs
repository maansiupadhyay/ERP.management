namespace CollegeERP.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? HeadOfDepartmentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Faculty? HeadOfDepartment { get; set; }
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
