using CollegeERP.Domain.Entities;
using CollegeERP.Domain.Enums;
using CollegeERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CollegeERP.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CollegeERPDbContext>();
        var isSqlite = context.Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;
        if (isSqlite)
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            await context.Database.MigrateAsync();
        }

        if (await context.Users.AnyAsync()) return;

        // Admin user
        var admin = new User { Email = "admin@college.edu", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = UserRole.Admin };
        context.Users.Add(admin);
        await context.SaveChangesAsync();

        // Departments
        var cse = new Department { Name = "Computer Science & Engineering", Code = "CSE", Description = "Department of Computer Science" };
        var ece = new Department { Name = "Electronics & Communication", Code = "ECE", Description = "Department of Electronics" };
        var me = new Department { Name = "Mechanical Engineering", Code = "ME", Description = "Department of Mechanical Engineering" };
        var ce = new Department { Name = "Civil Engineering", Code = "CE", Description = "Department of Civil Engineering" };
        context.Departments.AddRange(cse, ece, me, ce);
        await context.SaveChangesAsync();

        // Faculty
        var facultyData = new[]
        {
            (Email: "rajesh.kumar@college.edu", First: "Rajesh", Last: "Kumar", EmpId: "FAC001", DeptId: cse.Id, Desig: "Professor", Spec: "AI & ML"),
            (Email: "priya.sharma@college.edu", First: "Priya", Last: "Sharma", EmpId: "FAC002", DeptId: cse.Id, Desig: "Associate Professor", Spec: "Database Systems"),
            (Email: "amit.singh@college.edu", First: "Amit", Last: "Singh", EmpId: "FAC003", DeptId: ece.Id, Desig: "Professor", Spec: "VLSI Design"),
            (Email: "neha.gupta@college.edu", First: "Neha", Last: "Gupta", EmpId: "FAC004", DeptId: me.Id, Desig: "Assistant Professor", Spec: "Thermodynamics"),
        };

        var faculties = new List<Faculty>();
        foreach (var fd in facultyData)
        {
            var user = new User { Email = fd.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Faculty@123"), Role = UserRole.Faculty };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var faculty = new Faculty { UserId = user.Id, EmployeeId = fd.EmpId, FirstName = fd.First, LastName = fd.Last, DepartmentId = fd.DeptId, Designation = fd.Desig, Specialization = fd.Spec, Phone = "9876543210" };
            context.Faculty.Add(faculty);
            await context.SaveChangesAsync();
            faculties.Add(faculty);
        }

        cse.HeadOfDepartmentId = faculties[0].Id;
        ece.HeadOfDepartmentId = faculties[2].Id;
        await context.SaveChangesAsync();

        // Courses
        var courses = new[]
        {
            new Course { Name = "Data Structures", Code = "CS201", DepartmentId = cse.Id, Credits = 4, Semester = 3, Description = "Arrays, linked lists, trees, graphs" },
            new Course { Name = "Database Management", Code = "CS301", DepartmentId = cse.Id, Credits = 3, Semester = 5, Description = "SQL, normalization, transactions" },
            new Course { Name = "Operating Systems", Code = "CS302", DepartmentId = cse.Id, Credits = 4, Semester = 5, Description = "Process management, memory, file systems" },
            new Course { Name = "Digital Electronics", Code = "EC201", DepartmentId = ece.Id, Credits = 3, Semester = 3, Description = "Logic gates, flip-flops, counters" },
            new Course { Name = "Thermodynamics", Code = "ME201", DepartmentId = me.Id, Credits = 4, Semester = 3, Description = "Laws of thermodynamics, entropy" },
        };
        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();

        // Assign faculty to courses
        context.CourseFaculties.AddRange(
            new CourseFaculty { CourseId = courses[0].Id, FacultyId = faculties[0].Id, AcademicYear = "2025-26", Semester = 3 },
            new CourseFaculty { CourseId = courses[1].Id, FacultyId = faculties[1].Id, AcademicYear = "2025-26", Semester = 5 },
            new CourseFaculty { CourseId = courses[2].Id, FacultyId = faculties[0].Id, AcademicYear = "2025-26", Semester = 5 },
            new CourseFaculty { CourseId = courses[3].Id, FacultyId = faculties[2].Id, AcademicYear = "2025-26", Semester = 3 },
            new CourseFaculty { CourseId = courses[4].Id, FacultyId = faculties[3].Id, AcademicYear = "2025-26", Semester = 3 }
        );
        await context.SaveChangesAsync();

        // Students
        var studentData = new[]
        {
            (Email: "aarav.patel@student.edu", First: "Aarav", Last: "Patel", Enroll: "STU2025001", DeptId: cse.Id, Sem: 3),
            (Email: "diya.singh@student.edu", First: "Diya", Last: "Singh", Enroll: "STU2025002", DeptId: cse.Id, Sem: 3),
            (Email: "vivaan.sharma@student.edu", First: "Vivaan", Last: "Sharma", Enroll: "STU2025003", DeptId: cse.Id, Sem: 5),
            (Email: "ananya.gupta@student.edu", First: "Ananya", Last: "Gupta", Enroll: "STU2025004", DeptId: cse.Id, Sem: 5),
            (Email: "arjun.reddy@student.edu", First: "Arjun", Last: "Reddy", Enroll: "STU2025005", DeptId: ece.Id, Sem: 3),
            (Email: "ishita.verma@student.edu", First: "Ishita", Last: "Verma", Enroll: "STU2025006", DeptId: ece.Id, Sem: 3),
            (Email: "rohan.joshi@student.edu", First: "Rohan", Last: "Joshi", Enroll: "STU2025007", DeptId: me.Id, Sem: 3),
            (Email: "sneha.agarwal@student.edu", First: "Sneha", Last: "Agarwal", Enroll: "STU2025008", DeptId: me.Id, Sem: 3),
        };

        foreach (var sd in studentData)
        {
            var user = new User { Email = sd.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"), Role = UserRole.Student };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            context.Students.Add(new Student
            {
                UserId = user.Id, EnrollmentNumber = sd.Enroll,
                FirstName = sd.First, LastName = sd.Last,
                DepartmentId = sd.DeptId, Semester = sd.Sem, Section = "A",
                DateOfBirth = new DateTime(2003, 5, 15), Phone = "9876543210"
            });
        }
        await context.SaveChangesAsync();

        // Timetable
        var timetables = new[]
        {
            new Timetable { CourseId = courses[0].Id, FacultyId = faculties[0].Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Room = "CS-101", Section = "A", AcademicYear = "2025-26", Semester = 3 },
            new Timetable { CourseId = courses[0].Id, FacultyId = faculties[0].Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0), Room = "CS-101", Section = "A", AcademicYear = "2025-26", Semester = 3 },
            new Timetable { CourseId = courses[1].Id, FacultyId = faculties[1].Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(12, 0, 0), Room = "CS-201", Section = "A", AcademicYear = "2025-26", Semester = 5 },
            new Timetable { CourseId = courses[3].Id, FacultyId = faculties[2].Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), Room = "EC-101", Section = "A", AcademicYear = "2025-26", Semester = 3 },
            new Timetable { CourseId = courses[4].Id, FacultyId = faculties[3].Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 0, 0), Room = "ME-101", Section = "A", AcademicYear = "2025-26", Semester = 3 },
        };
        context.Timetables.AddRange(timetables);
        await context.SaveChangesAsync();
    }
}
