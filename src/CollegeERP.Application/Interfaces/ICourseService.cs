using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task<CourseDto?> GetByIdAsync(int id);
    Task<IEnumerable<CourseDto>> GetByDepartmentAsync(int departmentId);
    Task<CourseDto> CreateAsync(CreateCourseRequest request);
    Task<CourseDto?> UpdateAsync(int id, CreateCourseRequest request);
    Task<bool> DeleteAsync(int id);
    Task<bool> AssignFacultyAsync(AssignFacultyRequest request);
}
