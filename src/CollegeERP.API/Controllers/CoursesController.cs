using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _svc;
    public CoursesController(ICourseService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IEnumerable<CourseDto>>.Ok(await _svc.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _svc.GetByIdAsync(id);
        return c == null ? NotFound(ApiResponse<CourseDto>.Fail("Not found")) : Ok(ApiResponse<CourseDto>.Ok(c));
    }

    [HttpGet("department/{departmentId}")]
    public async Task<IActionResult> GetByDept(int departmentId)
        => Ok(ApiResponse<IEnumerable<CourseDto>>.Ok(await _svc.GetByDepartmentAsync(departmentId)));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest req)
        => Ok(ApiResponse<CourseDto>.Ok(await _svc.CreateAsync(req), "Created"));

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCourseRequest req)
    {
        var c = await _svc.UpdateAsync(id, req);
        return c == null ? NotFound(ApiResponse<CourseDto>.Fail("Not found")) : Ok(ApiResponse<CourseDto>.Ok(c, "Updated"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? Ok(ApiResponse<bool>.Ok(true, "Deleted")) : NotFound(ApiResponse<bool>.Fail("Not found"));

    [HttpPost("assign-faculty")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignFaculty([FromBody] AssignFacultyRequest req)
        => await _svc.AssignFacultyAsync(req) ? Ok(ApiResponse<bool>.Ok(true, "Faculty assigned")) : BadRequest(ApiResponse<bool>.Fail("Already assigned"));
}
