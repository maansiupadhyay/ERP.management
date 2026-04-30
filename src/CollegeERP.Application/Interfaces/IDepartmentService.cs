using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request);
    Task<DepartmentDto?> UpdateAsync(int id, CreateDepartmentRequest request);
    Task<bool> DeleteAsync(int id);
}
