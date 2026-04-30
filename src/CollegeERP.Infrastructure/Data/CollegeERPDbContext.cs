using CollegeERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Infrastructure.Data;

public class CollegeERPDbContext : DbContext
{
    public CollegeERPDbContext(DbContextOptions<CollegeERPDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Faculty> Faculty => Set<Faculty>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseFaculty> CourseFaculties => Set<CourseFaculty>();
    public DbSet<Timetable> Timetables => Set<Timetable>();
    public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.HasOne(u => u.Student).WithOne(s => s.User).HasForeignKey<Student>(s => s.UserId);
            e.HasOne(u => u.Faculty).WithOne(f => f.User).HasForeignKey<Faculty>(f => f.UserId);
        });

        // Student
        modelBuilder.Entity<Student>(e =>
        {
            e.HasIndex(s => s.EnrollmentNumber).IsUnique();
            e.HasOne(s => s.Department).WithMany(d => d.Students).HasForeignKey(s => s.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        });

        // Faculty
        modelBuilder.Entity<Faculty>(e =>
        {
            e.HasIndex(f => f.EmployeeId).IsUnique();
            e.HasOne(f => f.Department).WithMany(d => d.Faculties).HasForeignKey(f => f.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        });

        // Department
        modelBuilder.Entity<Department>(e =>
        {
            e.HasIndex(d => d.Code).IsUnique();
            e.HasOne(d => d.HeadOfDepartment).WithMany().HasForeignKey(d => d.HeadOfDepartmentId).OnDelete(DeleteBehavior.SetNull);
        });

        // Course
        modelBuilder.Entity<Course>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.HasOne(c => c.Department).WithMany(d => d.Courses).HasForeignKey(c => c.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        });

        // CourseFaculty
        modelBuilder.Entity<CourseFaculty>(e =>
        {
            e.HasIndex(cf => new { cf.CourseId, cf.FacultyId, cf.AcademicYear }).IsUnique();
            e.HasOne(cf => cf.Course).WithMany(c => c.CourseFaculties).HasForeignKey(cf => cf.CourseId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(cf => cf.Faculty).WithMany(f => f.CourseFaculties).HasForeignKey(cf => cf.FacultyId).OnDelete(DeleteBehavior.Restrict);
        });

        // Timetable
        modelBuilder.Entity<Timetable>(e =>
        {
            e.HasOne(t => t.Course).WithMany(c => c.Timetables).HasForeignKey(t => t.CourseId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Faculty).WithMany(f => f.Timetables).HasForeignKey(t => t.FacultyId).OnDelete(DeleteBehavior.Restrict);
        });

        // AttendanceSession
        modelBuilder.Entity<AttendanceSession>(e =>
        {
            e.HasOne(a => a.Course).WithMany(c => c.AttendanceSessions).HasForeignKey(a => a.CourseId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Faculty).WithMany(f => f.AttendanceSessions).HasForeignKey(a => a.FacultyId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Timetable).WithMany().HasForeignKey(a => a.TimetableId).OnDelete(DeleteBehavior.SetNull);
        });

        // AttendanceRecord
        modelBuilder.Entity<AttendanceRecord>(e =>
        {
            e.HasIndex(r => new { r.SessionId, r.StudentId }).IsUnique();
            e.HasOne(r => r.Session).WithMany(s => s.AttendanceRecords).HasForeignKey(r => r.SessionId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Student).WithMany(s => s.AttendanceRecords).HasForeignKey(r => r.StudentId).OnDelete(DeleteBehavior.Restrict);
        });

        // Exam
        modelBuilder.Entity<Exam>(e =>
        {
            e.HasOne(ex => ex.Course).WithMany(c => c.Exams).HasForeignKey(ex => ex.CourseId).OnDelete(DeleteBehavior.Restrict);
        });

        // ExamResult
        modelBuilder.Entity<ExamResult>(e =>
        {
            e.HasIndex(r => new { r.ExamId, r.StudentId }).IsUnique();
            e.HasOne(r => r.Exam).WithMany(ex => ex.ExamResults).HasForeignKey(r => r.ExamId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Student).WithMany(s => s.ExamResults).HasForeignKey(r => r.StudentId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
