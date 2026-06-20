using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.DTOs;
using StudentSaaS.API.Models;
using StudentSaaS.API.Services;
using StudentSaaS.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace StudentSaaS.API.Controllers;

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

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var exists = await _context.Users
            .AnyAsync(x => x.Email == dto.Email);

        if (exists)
            return BadRequest("Email already exists");

        // Ensure we have a default institute
        var institute = await _context.Institutes.FirstOrDefaultAsync();
        if (institute == null)
        {
            institute = new Institute
            {
                Name = "Sivaram Kalaikoodam Art Academy",
                Phone = dto.Phone,
                IsActive = true
            };
            _context.Institutes.Add(institute);
            await _context.SaveChangesAsync();
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = PasswordHelper.Hash(dto.Password),
            Role = dto.Role,
            Phone = dto.Phone,
            InstituteId = dto.InstituteId ?? institute.Id,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // If user is a Student, create Student record automatically
        if (dto.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
        {
            var student = new Student
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                InstituteId = user.InstituteId.Value,
                AdmissionDate = DateTime.UtcNow,
                Status = "Active",
                Course = "Pencil Drawing", // Default
                Batch = "5:00 PM - 6:00 PM" // Default
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }
        // If user is a Teacher, create Staff record automatically
        else if (dto.Role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = new Staff
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Teacher",
                InstituteId = user.InstituteId.Value,
                JoiningDate = DateTime.UtcNow,
                ActiveStatus = true,
                Subject = "General Art"
            };
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
        }

        return Ok(user);
    }

    [AllowAnonymous]
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

        if (!user.IsActive)
            return BadRequest("User account is inactive");

        // 🔐 Generate JWT & Refresh Token
        var token = _jwt.GenerateToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        var ipAddress = Request.Headers.ContainsKey("X-Forwarded-For") 
            ? Request.Headers["X-Forwarded-For"].ToString() 
            : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var dbRefreshToken = new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            UserId = user.Id
        };

        _context.RefreshTokens.Add(dbRefreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            token = token,
            refreshToken = refreshToken,
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

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        try
        {
            var principal = _jwt.GetPrincipalFromExpiredToken(dto.Token);
            var userIdStr = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return BadRequest("Invalid token claims");

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || !user.IsActive)
                return Unauthorized("User not found or inactive");

            var ipAddress = Request.Headers.ContainsKey("X-Forwarded-For") 
                ? Request.Headers["X-Forwarded-For"].ToString() 
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var existingToken = user.RefreshTokens.FirstOrDefault(t => t.Token == dto.RefreshToken);
            if (existingToken == null || !existingToken.IsActive)
                return Unauthorized("Invalid refresh token");

            // Revoke current refresh token
            existingToken.Revoked = DateTime.UtcNow;
            existingToken.RevokedByIp = ipAddress;

            // Generate new tokens
            var newJwtToken = _jwt.GenerateToken(user);
            var newRefreshToken = _jwt.GenerateRefreshToken();

            // Save new refresh token
            var dbRefreshToken = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = user.Id
            };
            _context.RefreshTokens.Add(dbRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken,
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
        catch (Exception ex)
        {
            return BadRequest($"Refresh token failed: {ex.Message}");
        }
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
    {
        var userId = ClaimsHelper.GetUserId(User);
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("User not found");

        var token = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
        if (token == null)
            return NotFound("Token not found");

        if (!token.IsActive)
            return BadRequest("Token is already inactive");

        var ipAddress = Request.Headers.ContainsKey("X-Forwarded-For") 
            ? Request.Headers["X-Forwarded-For"].ToString() 
            : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Token revoked successfully." });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = ClaimsHelper.GetUserId(User);
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found");

        var currentHashed = PasswordHelper.Hash(dto.CurrentPassword);
        if (user.PasswordHash != currentHashed)
            return BadRequest("Incorrect current password");

        user.PasswordHash = PasswordHelper.Hash(dto.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password changed successfully." });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
        {
            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        var token = new Random().Next(100000, 999999).ToString();
        user.ResetToken = token;
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = "Reset token generated successfully.",
            token = token
        });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => 
            u.ResetToken == dto.Token && 
            u.ResetTokenExpiry > DateTime.UtcNow);

        if (user == null)
        {
            return BadRequest("Invalid or expired reset token.");
        }

        user.PasswordHash = PasswordHelper.Hash(dto.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password has been reset successfully." });
    }
}