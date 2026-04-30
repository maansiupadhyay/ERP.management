using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class TimetableService : ITimetableService
{
    private readonly IGenericRepository<Timetable> _repo;

    public TimetableService(IGenericRepository<Timetable> repo) => _repo = repo;

    public async Task<IEnumerable<TimetableDto>> GetAllAsync()
    {
        var items = await _repo.Query()
            .Include(t => t.Course).Include(t => t.Faculty)
            .OrderBy(t => t.DayOfWeek).ThenBy(t => t.StartTime).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<TimetableDto?> GetByIdAsync(int id)
    {
        var t = await _repo.Query().Include(t => t.Course).Include(t => t.Faculty)
            .FirstOrDefaultAsync(t => t.Id == id);
        return t == null ? null : MapToDto(t);
    }

    public async Task<IEnumerable<TimetableDto>> GetByFacultyAsync(int facultyId)
    {
        var items = await _repo.Query()
            .Include(t => t.Course).Include(t => t.Faculty)
            .Where(t => t.FacultyId == facultyId)
            .OrderBy(t => t.DayOfWeek).ThenBy(t => t.StartTime).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<TimetableDto>> GetByDepartmentSemesterAsync(int departmentId, int semester)
    {
        var items = await _repo.Query()
            .Include(t => t.Course).Include(t => t.Faculty)
            .Where(t => t.Course.DepartmentId == departmentId && t.Semester == semester)
            .OrderBy(t => t.DayOfWeek).ThenBy(t => t.StartTime).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<TimetableDto> CreateAsync(CreateTimetableRequest request)
    {
        var t = new Timetable
        {
            CourseId = request.CourseId, FacultyId = request.FacultyId,
            DayOfWeek = Enum.Parse<DayOfWeek>(request.DayOfWeek),
            StartTime = TimeSpan.Parse(request.StartTime),
            EndTime = TimeSpan.Parse(request.EndTime),
            Room = request.Room, Section = request.Section,
            AcademicYear = request.AcademicYear, Semester = request.Semester
        };
        await _repo.AddAsync(t);
        return (await GetByIdAsync(t.Id))!;
    }

    public async Task<TimetableDto?> UpdateAsync(int id, CreateTimetableRequest request)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t == null) return null;
        t.CourseId = request.CourseId; t.FacultyId = request.FacultyId;
        t.DayOfWeek = Enum.Parse<DayOfWeek>(request.DayOfWeek);
        t.StartTime = TimeSpan.Parse(request.StartTime);
        t.EndTime = TimeSpan.Parse(request.EndTime);
        t.Room = request.Room; t.Section = request.Section;
        t.AcademicYear = request.AcademicYear; t.Semester = request.Semester;
        t.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(t);
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t == null) return false;
        await _repo.DeleteAsync(t);
        return true;
    }

    private static TimetableDto MapToDto(Timetable t) => new()
    {
        Id = t.Id, CourseId = t.CourseId,
        CourseName = t.Course?.Name ?? "", CourseCode = t.Course?.Code ?? "",
        FacultyId = t.FacultyId,
        FacultyName = t.Faculty != null ? $"{t.Faculty.FirstName} {t.Faculty.LastName}" : "",
        DayOfWeek = t.DayOfWeek.ToString(),
        StartTime = t.StartTime.ToString(@"hh\:mm"),
        EndTime = t.EndTime.ToString(@"hh\:mm"),
        Room = t.Room, Section = t.Section,
        AcademicYear = t.AcademicYear, Semester = t.Semester
    };
}
