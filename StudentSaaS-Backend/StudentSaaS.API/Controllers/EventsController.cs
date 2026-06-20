using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/events
    [HttpGet]
    public async Task<IActionResult> GetEvents(
        [FromQuery] int? year,
        [FromQuery] int instituteId = 1)
    {
        var query = _context.Events.Where(e => e.InstituteId == instituteId);

        if (year.HasValue)
        {
            query = query.Where(e => e.EventDate.Year == year.Value);
        }

        var events = await query
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/events/years
    [HttpGet("years")]
    public async Task<IActionResult> GetEventYears([FromQuery] int instituteId = 1)
    {
        var years = await _context.Events
            .Where(e => e.InstituteId == instituteId)
            .Select(e => e.EventDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();

        return Ok(years);
    }

    // GET: api/events/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
            return NotFound("Event not found");

        return Ok(ev);
    }

    // POST: api/events (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] Event model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Events.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = model.Id }, model);
    }

    // PUT: api/events/{id} (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event model)
    {
        if (id != model.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.Events.FindAsync(id);
        if (existing == null)
            return NotFound("Event not found");

        existing.Title = model.Title;
        existing.Description = model.Description;
        existing.Category = model.Category;
        existing.Location = model.Location;
        existing.EventDate = model.EventDate;
        if (!string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            existing.ImageUrl = model.ImageUrl;
        }

        _context.Events.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE: api/events/{id} (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
            return NotFound("Event not found");

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Event deleted successfully." });
    }
}
