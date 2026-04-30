using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetAdminStatsAsync();
    Task<FacultyDashboardDto> GetFacultyDashboardAsync(int facultyId);
    Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId);
}
