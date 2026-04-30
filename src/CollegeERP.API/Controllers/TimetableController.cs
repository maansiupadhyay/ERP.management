using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimetableController : ControllerBase
{
    private readonly ITimetableService _svc;
    public TimetableController(ITimetableService svc) => _svc = svc;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(ApiResponse<IEnumerable<TimetableDto>>.Ok(await _svc.GetAllAsync()));
    [HttpGet("{id}")] public async Task<IActionResult> GetById(int id)
    { var t = await _svc.GetByIdAsync(id); return t == null ? NotFound(ApiResponse<TimetableDto>.Fail("Not found")) : Ok(ApiResponse<TimetableDto>.Ok(t)); }
    [HttpGet("faculty/{facultyId}")] public async Task<IActionResult> GetByFaculty(int facultyId) => Ok(ApiResponse<IEnumerable<TimetableDto>>.Ok(await _svc.GetByFacultyAsync(facultyId)));
    [HttpGet("department/{deptId}/semester/{sem}")] public async Task<IActionResult> GetByDeptSem(int deptId, int sem) => Ok(ApiResponse<IEnumerable<TimetableDto>>.Ok(await _svc.GetByDepartmentSemesterAsync(deptId, sem)));
    [HttpPost][Authorize(Roles = "Admin")] public async Task<IActionResult> Create([FromBody] CreateTimetableRequest req) => Ok(ApiResponse<TimetableDto>.Ok(await _svc.CreateAsync(req), "Created"));
    [HttpPut("{id}")][Authorize(Roles = "Admin")] public async Task<IActionResult> Update(int id, [FromBody] CreateTimetableRequest req)
    { var t = await _svc.UpdateAsync(id, req); return t == null ? NotFound(ApiResponse<TimetableDto>.Fail("Not found")) : Ok(ApiResponse<TimetableDto>.Ok(t, "Updated")); }
    [HttpDelete("{id}")][Authorize(Roles = "Admin")] public async Task<IActionResult> Delete(int id) => await _svc.DeleteAsync(id) ? Ok(ApiResponse<bool>.Ok(true, "Deleted")) : NotFound(ApiResponse<bool>.Fail("Not found"));
}
