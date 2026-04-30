using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface IFacultyService
{
    Task<IEnumerable<FacultyDto>> GetAllAsync();
    Task<FacultyDto?> GetByIdAsync(int id);
    Task<IEnumerable<FacultyDto>> GetByDepartmentAsync(int departmentId);
    Task<FacultyDto> CreateAsync(CreateFacultyRequest request);
    Task<FacultyDto?> UpdateAsync(int id, UpdateFacultyRequest request);
    Task<bool> DeleteAsync(int id);
}
