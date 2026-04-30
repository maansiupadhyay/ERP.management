using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _svc;
    public DepartmentsController(IDepartmentService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IEnumerable<DepartmentDto>>.Ok(await _svc.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var d = await _svc.GetByIdAsync(id);
        return d == null ? NotFound(ApiResponse<DepartmentDto>.Fail("Not found")) : Ok(ApiResponse<DepartmentDto>.Ok(d));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest req)
        => Ok(ApiResponse<DepartmentDto>.Ok(await _svc.CreateAsync(req), "Department created"));

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateDepartmentRequest req)
    {
        var d = await _svc.UpdateAsync(id, req);
        return d == null ? NotFound(ApiResponse<DepartmentDto>.Fail("Not found")) : Ok(ApiResponse<DepartmentDto>.Ok(d, "Updated"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? Ok(ApiResponse<bool>.Ok(true, "Deleted")) : NotFound(ApiResponse<bool>.Fail("Not found"));
}
