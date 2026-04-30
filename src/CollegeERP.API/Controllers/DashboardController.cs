using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _svc;
    public DashboardController(IDashboardService svc) => _svc = svc;

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminStats()
        => Ok(ApiResponse<DashboardStatsDto>.Ok(await _svc.GetAdminStatsAsync()));

    [HttpGet("faculty/{facultyId}")]
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> GetFacultyDashboard(int facultyId)
        => Ok(ApiResponse<FacultyDashboardDto>.Ok(await _svc.GetFacultyDashboardAsync(facultyId)));

    [HttpGet("student/{studentId}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> GetStudentDashboard(int studentId)
        => Ok(ApiResponse<StudentDashboardDto>.Ok(await _svc.GetStudentDashboardAsync(studentId)));
}
