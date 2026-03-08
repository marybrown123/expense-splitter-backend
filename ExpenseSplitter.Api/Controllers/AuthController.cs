using BCrypt.Net;
using ExpenseSplitter.Api.DTOs.Requests;
using ExpenseSplitter.Api.DTOs.Responses;
using ExpenseSplitter.Api.Services;
using ExpenseSplitter.Application.Entities;
using ExpenseSplitter.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext db, JwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();
        var username = request.Username.Trim();

        var userExists = await _db.Users.AnyAsync(u => u.Email == email);
        if (userExists)
        {
            return BadRequest(new { message = "User with this email already exists." });
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return StatusCode(StatusCodes.Status201Created, new AuthResponse
        {
            Token = token
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token
        });
    }
}