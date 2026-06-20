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
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ContactsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int instituteId = 1)
    {
        var list = await _context.Contacts
            .Where(c => c.InstituteId == instituteId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var dtos = _mapper.Map<List<ContactDto>>(list);
        return Ok(dtos);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null || contact.IsDeleted)
            return NotFound("Contact message not found");

        contact.IsDeleted = true;
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Contact message removed successfully" });
    }
}
