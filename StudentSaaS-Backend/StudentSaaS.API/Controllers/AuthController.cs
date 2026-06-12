using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.DTOs;
using StudentSaaS.API.Models;
using StudentSaaS.API.Services;
using StudentSaaS.API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace StudentSaaS.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwt;

    public AuthController(ApplicationDbContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var exists = await _context.Users
            .AnyAsync(x => x.Email == dto.Email);

        if (exists)
            return BadRequest("Email already exists");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = PasswordHelper.Hash(dto.Password),
            Role = dto.Role,
            InstituteId = dto.InstituteId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var hashedPassword = PasswordHelper.Hash(dto.Password);

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email == dto.Email &&
                x.PasswordHash == hashedPassword);

        if (user == null)
            return Unauthorized("Invalid email or password");

        // 🔐 Generate JWT
        var token = _jwt.GenerateToken(user);

        return Ok(new
        {
            token = token,
            user = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.InstituteId 
            }
        });
    }
}