using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _svc;
    public AttendanceController(IAttendanceService svc) => _svc = svc;

    [HttpPost("sessions")]
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> CreateSession([FromBody] CreateAttendanceSessionRequest req)
        => Ok(ApiResponse<AttendanceSessionDto>.Ok(await _svc.CreateSessionAsync(req), "Session created"));

    [HttpPost("mark")]
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceRequest req)
        => await _svc.MarkAttendanceAsync(req) ? Ok(ApiResponse<bool>.Ok(true, "Attendance marked")) : BadRequest(ApiResponse<bool>.Fail("Failed to mark"));

    [HttpPut("sessions/{sessionId}/close")]
    [Authorize(Roles = "Admin,Faculty")]
    public async Task<IActionResult> CloseSession(int sessionId)
        => await _svc.CloseSessionAsync(sessionId) ? Ok(ApiResponse<bool>.Ok(true, "Session closed")) : NotFound(ApiResponse<bool>.Fail("Not found"));

    [HttpGet("sessions")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date, [FromQuery] int? facultyId)
        => Ok(ApiResponse<IEnumerable<AttendanceSessionDto>>.Ok(await _svc.GetSessionsByDateAsync(date, facultyId)));

    [HttpGet("sessions/course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
        => Ok(ApiResponse<IEnumerable<AttendanceSessionDto>>.Ok(await _svc.GetSessionsByCourseAsync(courseId)));

    [HttpGet("sessions/{sessionId}/records")]
    public async Task<IActionResult> GetRecords(int sessionId)
        => Ok(ApiResponse<IEnumerable<AttendanceRecordDto>>.Ok(await _svc.GetSessionRecordsAsync(sessionId)));

    [HttpGet("report/{courseId}")]
    public async Task<IActionResult> GetReport(int courseId, [FromQuery] int? studentId)
        => Ok(ApiResponse<AttendanceReportDto>.Ok(await _svc.GetAttendanceReportAsync(courseId, studentId)));

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentSummary(int studentId)
        => Ok(ApiResponse<StudentAttendanceSummaryDto>.Ok(await _svc.GetStudentAttendanceSummaryAsync(studentId)));
}
