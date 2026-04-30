using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using CollegeERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class FacultyService : IFacultyService
{
    private readonly IGenericRepository<Faculty> _facultyRepo;
    private readonly IGenericRepository<User> _userRepo;

    public FacultyService(IGenericRepository<Faculty> facultyRepo, IGenericRepository<User> userRepo)
    {
        _facultyRepo = facultyRepo;
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<FacultyDto>> GetAllAsync()
    {
        var faculty = await _facultyRepo.Query()
            .Include(f => f.Department).Include(f => f.User)
            .OrderBy(f => f.FirstName).ToListAsync();
        return faculty.Select(MapToDto);
    }

    public async Task<FacultyDto?> GetByIdAsync(int id)
    {
        var f = await _facultyRepo.Query()
            .Include(f => f.Department).Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);
        return f == null ? null : MapToDto(f);
    }

    public async Task<IEnumerable<FacultyDto>> GetByDepartmentAsync(int departmentId)
    {
        var faculty = await _facultyRepo.Query()
            .Include(f => f.Department).Include(f => f.User)
            .Where(f => f.DepartmentId == departmentId)
            .OrderBy(f => f.FirstName).ToListAsync();
        return faculty.Select(MapToDto);
    }

    public async Task<FacultyDto> CreateAsync(CreateFacultyRequest request)
    {
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Faculty,
            IsActive = true
        };
        await _userRepo.AddAsync(user);

        var faculty = new Faculty
        {
            UserId = user.Id,
            EmployeeId = request.EmployeeId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DepartmentId = request.DepartmentId,
            Designation = request.Designation,
            Specialization = request.Specialization,
            Phone = request.Phone
        };
        await _facultyRepo.AddAsync(faculty);
        return (await GetByIdAsync(faculty.Id))!;
    }

    public async Task<FacultyDto?> UpdateAsync(int id, UpdateFacultyRequest request)
    {
        var f = await _facultyRepo.Query()
            .Include(f => f.Department).Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (f == null) return null;

        if (request.FirstName != null) f.FirstName = request.FirstName;
        if (request.LastName != null) f.LastName = request.LastName;
        if (request.DepartmentId.HasValue) f.DepartmentId = request.DepartmentId.Value;
        if (request.Designation != null) f.Designation = request.Designation;
        if (request.Specialization != null) f.Specialization = request.Specialization;
        if (request.Phone != null) f.Phone = request.Phone;
        f.UpdatedAt = DateTime.UtcNow;

        await _facultyRepo.UpdateAsync(f);
        return MapToDto(f);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var f = await _facultyRepo.GetByIdAsync(id);
        if (f == null) return false;
        var user = await _userRepo.GetByIdAsync(f.UserId);
        if (user != null) { user.IsActive = false; await _userRepo.UpdateAsync(user); }
        await _facultyRepo.DeleteAsync(f);
        return true;
    }

    private static FacultyDto MapToDto(Faculty f) => new()
    {
        Id = f.Id, UserId = f.UserId, EmployeeId = f.EmployeeId,
        FirstName = f.FirstName, LastName = f.LastName,
        DepartmentId = f.DepartmentId, DepartmentName = f.Department?.Name ?? "",
        Designation = f.Designation, Specialization = f.Specialization,
        Phone = f.Phone, JoinDate = f.JoinDate, Email = f.User?.Email ?? ""
    };
}
