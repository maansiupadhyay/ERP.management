using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacultyController : ControllerBase
{
    private readonly IFacultyService _svc;
    public FacultyController(IFacultyService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IEnumerable<FacultyDto>>.Ok(await _svc.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var f = await _svc.GetByIdAsync(id);
        return f == null ? NotFound(ApiResponse<FacultyDto>.Fail("Faculty not found")) : Ok(ApiResponse<FacultyDto>.Ok(f));
    }

    [HttpGet("department/{departmentId}")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
        => Ok(ApiResponse<IEnumerable<FacultyDto>>.Ok(await _svc.GetByDepartmentAsync(departmentId)));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateFacultyRequest request)
        => Ok(ApiResponse<FacultyDto>.Ok(await _svc.CreateAsync(request), "Faculty created"));

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFacultyRequest request)
    {
        var f = await _svc.UpdateAsync(id, request);
        return f == null ? NotFound(ApiResponse<FacultyDto>.Fail("Faculty not found")) : Ok(ApiResponse<FacultyDto>.Ok(f, "Faculty updated"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? Ok(ApiResponse<bool>.Ok(true, "Faculty deleted")) : NotFound(ApiResponse<bool>.Fail("Faculty not found"));
}
