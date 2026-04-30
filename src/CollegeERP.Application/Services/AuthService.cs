using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using CollegeERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<Student> _studentRepo;
    private readonly IGenericRepository<Faculty> _facultyRepo;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(
        IGenericRepository<User> userRepo,
        IGenericRepository<Student> studentRepo,
        IGenericRepository<Faculty> facultyRepo,
        IJwtTokenGenerator tokenGenerator)
    {
        _userRepo = userRepo;
        _studentRepo = studentRepo;
        _facultyRepo = facultyRepo;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = (await _userRepo.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return new LoginResponse { Success = false, Message = "Invalid email or password" };

        if (!user.IsActive)
            return new LoginResponse { Success = false, Message = "Account is deactivated" };

        var token = _tokenGenerator.GenerateToken(user);
        var userDto = await BuildUserDto(user);

        return new LoginResponse { Success = true, Token = token, User = userDto };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = (await _userRepo.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
        if (existing != null)
            return new LoginResponse { Success = false, Message = "Email already registered" };

        var role = Enum.Parse<UserRole>(request.Role, true);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role, IsActive = true
        };
        await _userRepo.AddAsync(user);

        if (role == UserRole.Student && request.DepartmentId.HasValue)
        {
            await _studentRepo.AddAsync(new Student
            {
                UserId = user.Id,
                EnrollmentNumber = request.EnrollmentNumber ?? $"STU{user.Id:D6}",
                FirstName = request.FirstName, LastName = request.LastName,
                DepartmentId = request.DepartmentId.Value,
                Semester = request.Semester ?? 1,
                Section = request.Section ?? "A",
                Phone = request.Phone ?? ""
            });
        }
        else if (role == UserRole.Faculty && request.DepartmentId.HasValue)
        {
            await _facultyRepo.AddAsync(new Faculty
            {
                UserId = user.Id,
                EmployeeId = request.EmployeeId ?? $"FAC{user.Id:D6}",
                FirstName = request.FirstName, LastName = request.LastName,
                DepartmentId = request.DepartmentId.Value,
                Designation = request.Designation ?? "Assistant Professor",
                Phone = request.Phone ?? ""
            });
        }

        var token = _tokenGenerator.GenerateToken(user);
        var userDto = await BuildUserDto(user);
        return new LoginResponse { Success = true, Token = token, User = userDto };
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) return null;
        return await BuildUserDto(user);
    }

    private async Task<UserDto> BuildUserDto(User user)
    {
        var dto = new UserDto
        {
            Id = user.Id, Email = user.Email,
            Role = user.Role.ToString(), IsActive = user.IsActive
        };

        if (user.Role == UserRole.Student)
        {
            var s = (await _studentRepo.FindAsync(s => s.UserId == user.Id)).FirstOrDefault();
            if (s != null) { dto.FullName = $"{s.FirstName} {s.LastName}"; dto.ProfileId = s.Id; }
        }
        else if (user.Role == UserRole.Faculty)
        {
            var f = (await _facultyRepo.FindAsync(f => f.UserId == user.Id)).FirstOrDefault();
            if (f != null) { dto.FullName = $"{f.FirstName} {f.LastName}"; dto.ProfileId = f.Id; }
        }
        else
        {
            dto.FullName = "Administrator";
        }
        return dto;
    }
}

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
