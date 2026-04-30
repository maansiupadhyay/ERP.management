using CollegeERP.API.Models;
using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollegeERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success) return BadRequest(ApiResponse<LoginResponse>.Fail(result.Message ?? "Login failed"));
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success) return BadRequest(ApiResponse<LoginResponse>.Fail(result.Message ?? "Registration failed"));
        return Ok(ApiResponse<LoginResponse>.Ok(result));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized(ApiResponse<UserDto>.Fail("User not authenticated"));

        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null) return NotFound(ApiResponse<UserDto>.Fail("User not found"));
        return Ok(ApiResponse<UserDto>.Ok(user));
    }
}
