using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GalleryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GalleryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/gallery
    [HttpGet]
    public async Task<IActionResult> GetGallery(
        [FromQuery] string? category,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] int instituteId = 1)
    {
        var query = _context.GalleryImages.Where(g => g.InstituteId == instituteId);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(g => g.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(g => g.Title.Contains(search));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(g => g.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            items
        });
    }

    // GET: api/gallery/categories
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] int instituteId = 1)
    {
        var categories = await _context.GalleryImages
            .Where(g => g.InstituteId == instituteId)
            .Select(g => g.Category)
            .Distinct()
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/gallery/featured
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int instituteId = 1)
    {
        var featured = await _context.GalleryImages
            .Where(g => g.InstituteId == instituteId && g.IsFeatured)
            .Take(6)
            .ToListAsync();

        return Ok(featured);
    }

    // GET: api/gallery/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGalleryImage(int id)
    {
        var image = await _context.GalleryImages.FindAsync(id);
        if (image == null)
            return NotFound("Image not found");

        return Ok(image);
    }

    // POST: api/gallery (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateGalleryImage([FromBody] GalleryImage model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.GalleryImages.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGalleryImage), new { id = model.Id }, model);
    }

    // POST: api/gallery/upload (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        var fileUrl = "/uploads/" + uniqueFileName;
        return Ok(new { url = fileUrl });
    }

    // PUT: api/gallery/{id} (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGalleryImage(int id, [FromBody] GalleryImage model)
    {
        if (id != model.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.GalleryImages.FindAsync(id);
        if (existing == null)
            return NotFound("Image not found");

        existing.Title = model.Title;
        existing.Category = model.Category;
        existing.IsFeatured = model.IsFeatured;
        if (!string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            existing.ImageUrl = model.ImageUrl;
        }

        _context.GalleryImages.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE: api/gallery/{id} (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGalleryImage(int id)
    {
        var image = await _context.GalleryImages.FindAsync(id);
        if (image == null)
            return NotFound("Image not found");

        _context.GalleryImages.Remove(image);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Image deleted successfully." });
    }
}
