using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.DTOs;
using StudentSaaS.API.Helpers;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public NotificationsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] int instituteId = 1)
    {
        var userId = ClaimsHelper.GetUserId(User);

        var list = await _context.Notifications
            .Where(n => n.UserId == userId && n.InstituteId == instituteId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var dtos = _mapper.Map<List<NotificationDto>>(list);
        return Ok(dtos);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = ClaimsHelper.GetUserId(User);

        var notif = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && !n.IsDeleted);

        if (notif == null)
            return NotFound("Notification not found");

        notif.IsRead = true;
        _context.Notifications.Update(notif);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Notification marked as read" });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead([FromQuery] int instituteId = 1)
    {
        var userId = ClaimsHelper.GetUserId(User);

        var list = await _context.Notifications
            .Where(n => n.UserId == userId && n.InstituteId == instituteId && !n.IsRead && !n.IsDeleted)
            .ToListAsync();

        foreach (var item in list)
        {
            item.IsRead = true;
        }

        _context.Notifications.UpdateRange(list);
        await _context.SaveChangesAsync();

        return Ok(new { message = "All notifications marked as read" });
    }
}
