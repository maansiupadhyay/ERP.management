using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IGenericRepository<Department> _repo;

    public DepartmentService(IGenericRepository<Department> repo) => _repo = repo;

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var depts = await _repo.Query()
            .Include(d => d.Students).Include(d => d.Faculties).Include(d => d.Courses)
            .Include(d => d.HeadOfDepartment)
            .OrderBy(d => d.Name).ToListAsync();
        return depts.Select(MapToDto);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var d = await _repo.Query()
            .Include(d => d.Students).Include(d => d.Faculties).Include(d => d.Courses)
            .Include(d => d.HeadOfDepartment)
            .FirstOrDefaultAsync(d => d.Id == id);
        return d == null ? null : MapToDto(d);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request)
    {
        var dept = new Department
        {
            Name = request.Name, Code = request.Code,
            HeadOfDepartmentId = request.HeadOfDepartmentId,
            Description = request.Description
        };
        await _repo.AddAsync(dept);
        return (await GetByIdAsync(dept.Id))!;
    }

    public async Task<DepartmentDto?> UpdateAsync(int id, CreateDepartmentRequest request)
    {
        var d = await _repo.GetByIdAsync(id);
        if (d == null) return null;
        d.Name = request.Name; d.Code = request.Code;
        d.HeadOfDepartmentId = request.HeadOfDepartmentId;
        d.Description = request.Description;
        d.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(d);
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var d = await _repo.GetByIdAsync(id);
        if (d == null) return false;
        d.IsActive = false; d.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(d);
        return true;
    }

    private static DepartmentDto MapToDto(Department d) => new()
    {
        Id = d.Id, Name = d.Name, Code = d.Code,
        HeadOfDepartmentId = d.HeadOfDepartmentId,
        HodName = d.HeadOfDepartment != null ? $"{d.HeadOfDepartment.FirstName} {d.HeadOfDepartment.LastName}" : null,
        Description = d.Description, IsActive = d.IsActive,
        StudentCount = d.Students?.Count ?? 0,
        FacultyCount = d.Faculties?.Count ?? 0,
        CourseCount = d.Courses?.Count ?? 0
    };
}
