using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface ITimetableService
{
    Task<IEnumerable<TimetableDto>> GetAllAsync();
    Task<TimetableDto?> GetByIdAsync(int id);
    Task<IEnumerable<TimetableDto>> GetByFacultyAsync(int facultyId);
    Task<IEnumerable<TimetableDto>> GetByDepartmentSemesterAsync(int departmentId, int semester);
    Task<TimetableDto> CreateAsync(CreateTimetableRequest request);
    Task<TimetableDto?> UpdateAsync(int id, CreateTimetableRequest request);
    Task<bool> DeleteAsync(int id);
}
