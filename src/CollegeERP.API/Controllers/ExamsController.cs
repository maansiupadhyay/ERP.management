using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamsController : ControllerBase
{
    private readonly IExamService _svc;
    public ExamsController(IExamService svc) => _svc = svc;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(ApiResponse<IEnumerable<ExamDto>>.Ok(await _svc.GetAllAsync()));
    [HttpGet("{id}")] public async Task<IActionResult> GetById(int id)
    { var e = await _svc.GetByIdAsync(id); return e == null ? NotFound(ApiResponse<ExamDto>.Fail("Not found")) : Ok(ApiResponse<ExamDto>.Ok(e)); }
    [HttpGet("course/{courseId}")] public async Task<IActionResult> GetByCourse(int courseId) => Ok(ApiResponse<IEnumerable<ExamDto>>.Ok(await _svc.GetByCourseAsync(courseId)));
    [HttpPost][Authorize(Roles = "Admin,Faculty")] public async Task<IActionResult> Create([FromBody] CreateExamRequest req) => Ok(ApiResponse<ExamDto>.Ok(await _svc.CreateAsync(req), "Created"));
    [HttpPut("{id}")][Authorize(Roles = "Admin,Faculty")] public async Task<IActionResult> Update(int id, [FromBody] CreateExamRequest req)
    { var e = await _svc.UpdateAsync(id, req); return e == null ? NotFound(ApiResponse<ExamDto>.Fail("Not found")) : Ok(ApiResponse<ExamDto>.Ok(e, "Updated")); }
    [HttpDelete("{id}")][Authorize(Roles = "Admin")] public async Task<IActionResult> Delete(int id) => await _svc.DeleteAsync(id) ? Ok(ApiResponse<bool>.Ok(true, "Deleted")) : NotFound(ApiResponse<bool>.Fail("Not found"));
    [HttpPost("{examId}/results")][Authorize(Roles = "Admin,Faculty")] public async Task<IActionResult> RecordResults(int examId, [FromBody] List<RecordResultRequest> results)
        => await _svc.RecordResultsAsync(examId, results) ? Ok(ApiResponse<bool>.Ok(true, "Results recorded")) : BadRequest(ApiResponse<bool>.Fail("Failed"));
    [HttpGet("{examId}/results")] public async Task<IActionResult> GetResults(int examId) => Ok(ApiResponse<IEnumerable<ExamResultDto>>.Ok(await _svc.GetResultsAsync(examId)));
    [HttpGet("student/{studentId}/results")] public async Task<IActionResult> GetStudentResults(int studentId) => Ok(ApiResponse<IEnumerable<ExamResultDto>>.Ok(await _svc.GetStudentResultsAsync(studentId)));
}
