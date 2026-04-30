using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllAsync();
    Task<StudentDto?> GetByIdAsync(int id);
    Task<IEnumerable<StudentDto>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<StudentDto>> GetBySemesterAsync(int departmentId, int semester);
    Task<StudentDto> CreateAsync(CreateStudentRequest request);
    Task<StudentDto?> UpdateAsync(int id, UpdateStudentRequest request);
    Task<bool> DeleteAsync(int id);
    Task<StudentAttendanceSummaryDto> GetAttendanceSummaryAsync(int studentId);
}
