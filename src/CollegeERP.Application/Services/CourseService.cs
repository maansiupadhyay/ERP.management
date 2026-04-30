using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class CourseService : ICourseService
{
    private readonly IGenericRepository<Course> _courseRepo;
    private readonly IGenericRepository<CourseFaculty> _cfRepo;

    public CourseService(IGenericRepository<Course> courseRepo, IGenericRepository<CourseFaculty> cfRepo)
    {
        _courseRepo = courseRepo;
        _cfRepo = cfRepo;
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        var courses = await _courseRepo.Query()
            .Include(c => c.Department)
            .Include(c => c.CourseFaculties).ThenInclude(cf => cf.Faculty)
            .OrderBy(c => c.Name).ToListAsync();
        return courses.Select(MapToDto);
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var c = await _courseRepo.Query()
            .Include(c => c.Department)
            .Include(c => c.CourseFaculties).ThenInclude(cf => cf.Faculty)
            .FirstOrDefaultAsync(c => c.Id == id);
        return c == null ? null : MapToDto(c);
    }

    public async Task<IEnumerable<CourseDto>> GetByDepartmentAsync(int departmentId)
    {
        var courses = await _courseRepo.Query()
            .Include(c => c.Department)
            .Include(c => c.CourseFaculties).ThenInclude(cf => cf.Faculty)
            .Where(c => c.DepartmentId == departmentId)
            .OrderBy(c => c.Semester).ThenBy(c => c.Name).ToListAsync();
        return courses.Select(MapToDto);
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request)
    {
        var course = new Course
        {
            Name = request.Name, Code = request.Code,
            DepartmentId = request.DepartmentId, Credits = request.Credits,
            Semester = request.Semester, Description = request.Description
        };
        await _courseRepo.AddAsync(course);
        return (await GetByIdAsync(course.Id))!;
    }

    public async Task<CourseDto?> UpdateAsync(int id, CreateCourseRequest request)
    {
        var c = await _courseRepo.GetByIdAsync(id);
        if (c == null) return null;
        c.Name = request.Name; c.Code = request.Code;
        c.DepartmentId = request.DepartmentId; c.Credits = request.Credits;
        c.Semester = request.Semester; c.Description = request.Description;
        c.UpdatedAt = DateTime.UtcNow;
        await _courseRepo.UpdateAsync(c);
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await _courseRepo.GetByIdAsync(id);
        if (c == null) return false;
        c.IsActive = false; c.UpdatedAt = DateTime.UtcNow;
        await _courseRepo.UpdateAsync(c);
        return true;
    }

    public async Task<bool> AssignFacultyAsync(AssignFacultyRequest request)
    {
        var exists = await _cfRepo.Query()
            .AnyAsync(cf => cf.CourseId == request.CourseId && cf.FacultyId == request.FacultyId
                && cf.AcademicYear == request.AcademicYear);
        if (exists) return false;

        await _cfRepo.AddAsync(new CourseFaculty
        {
            CourseId = request.CourseId, FacultyId = request.FacultyId,
            AcademicYear = request.AcademicYear, Semester = request.Semester
        });
        return true;
    }

    private static CourseDto MapToDto(Course c) => new()
    {
        Id = c.Id, Name = c.Name, Code = c.Code,
        DepartmentId = c.DepartmentId, DepartmentName = c.Department?.Name ?? "",
        Credits = c.Credits, Semester = c.Semester, Description = c.Description,
        IsActive = c.IsActive,
        AssignedFaculty = c.CourseFaculties?.Select(cf =>
            $"{cf.Faculty.FirstName} {cf.Faculty.LastName}").ToList() ?? new()
    };
}
