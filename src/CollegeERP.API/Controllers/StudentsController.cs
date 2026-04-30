using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _svc;
    public StudentsController(IStudentService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IEnumerable<StudentDto>>.Ok(await _svc.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _svc.GetByIdAsync(id);
        return s == null ? NotFound(ApiResponse<StudentDto>.Fail("Student not found")) : Ok(ApiResponse<StudentDto>.Ok(s));
    }

    [HttpGet("department/{departmentId}")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
        => Ok(ApiResponse<IEnumerable<StudentDto>>.Ok(await _svc.GetByDepartmentAsync(departmentId)));

    [HttpGet("department/{departmentId}/semester/{semester}")]
    public async Task<IActionResult> GetBySemester(int departmentId, int semester)
        => Ok(ApiResponse<IEnumerable<StudentDto>>.Ok(await _svc.GetBySemesterAsync(departmentId, semester)));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
        => Ok(ApiResponse<StudentDto>.Ok(await _svc.CreateAsync(request), "Student created"));

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        var s = await _svc.UpdateAsync(id, request);
        return s == null ? NotFound(ApiResponse<StudentDto>.Fail("Student not found")) : Ok(ApiResponse<StudentDto>.Ok(s, "Student updated"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? Ok(ApiResponse<bool>.Ok(true, "Student deleted")) : NotFound(ApiResponse<bool>.Fail("Student not found"));

    [HttpGet("{id}/attendance")]
    public async Task<IActionResult> GetAttendance(int id)
        => Ok(ApiResponse<StudentAttendanceSummaryDto>.Ok(await _svc.GetAttendanceSummaryAsync(id)));
}
