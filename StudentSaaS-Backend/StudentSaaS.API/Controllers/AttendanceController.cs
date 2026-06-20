using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Models;
using StudentSaaS.API.Helpers;

namespace StudentSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AttendanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/attendance
    [HttpGet]
    public async Task<IActionResult> GetAttendance(
        [FromQuery] DateTime? date,
        [FromQuery] int? studentId,
        [FromQuery] int instituteId = 1)
    {
        var role = ClaimsHelper.GetRole(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var query = _context.Attendances.Where(x => x.InstituteId == instituteId && !x.IsDeleted);

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff != null)
            {
                var assignedBatches = await _context.Batches
                    .Where(b => b.TeacherId == staff.Id && !b.IsDeleted && b.IsActive)
                    .Select(b => b.BatchName)
                    .ToListAsync();
                
                var studentIds = await _context.Students
                    .Where(s => assignedBatches.Contains(s.Batch) && !s.IsDeleted)
                    .Select(s => s.Id)
                    .ToListAsync();

                query = query.Where(x => studentIds.Contains(x.StudentId));
            }
            else
            {
                query = query.Where(x => false);
            }
        }

        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            query = query.Where(x => x.Date.Date == targetDate);
        }

        if (studentId.HasValue)
        {
            query = query.Where(x => x.StudentId == studentId.Value);
        }

        var results = await query.OrderByDescending(x => x.Date).ToListAsync();
        return Ok(results);
    }

    // GET: api/attendance/students
    [HttpGet("students")]
    public async Task<IActionResult> GetStudentsForAttendance(
        [FromQuery] string? course,
        [FromQuery] string? batch,
        [FromQuery] int instituteId = 1)
    {
        var role = ClaimsHelper.GetRole(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var query = _context.Students.Where(s => s.InstituteId == instituteId && !s.IsDeleted && s.Status == "Active");

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff != null)
            {
                var assignedBatches = await _context.Batches
                    .Where(b => b.TeacherId == staff.Id && !b.IsDeleted && b.IsActive)
                    .Select(b => b.BatchName)
                    .ToListAsync();

                query = query.Where(s => assignedBatches.Contains(s.Batch));
            }
            else
            {
                query = query.Where(s => false);
            }
        }

        if (!string.IsNullOrWhiteSpace(course))
        {
            query = query.Where(s => s.Course == course);
        }

        if (!string.IsNullOrWhiteSpace(batch))
        {
            query = query.Where(s => s.Batch == batch);
        }

        var students = await query.OrderBy(s => s.Name).ToListAsync();
        return Ok(students);
    }

    // POST: api/attendance/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> TakeBulkAttendance([FromBody] BulkAttendanceDto dto)
    {
        if (dto == null || dto.Records == null || dto.Records.Count == 0)
        {
            return BadRequest("Invalid attendance records.");
        }

        var date = dto.Date.Date;

        foreach (var record in dto.Records)
        {
            var existing = await _context.Attendances
                .FirstOrDefaultAsync(x => x.StudentId == record.StudentId && x.Date.Date == date && x.InstituteId == dto.InstituteId && !x.IsDeleted);

            if (existing != null)
            {
                existing.Status = record.Status;
                existing.Remarks = record.Remarks ?? string.Empty;
                _context.Attendances.Update(existing);
            }
            else
            {
                var name = record.StudentName;
                if (string.IsNullOrWhiteSpace(name))
                {
                    var student = await _context.Students.FindAsync(record.StudentId);
                    name = student?.Name ?? "Unknown Student";
                }

                var attendance = new Attendance
                {
                    StudentId = record.StudentId,
                    StudentName = name,
                    Date = date,
                    Status = record.Status,
                    Remarks = record.Remarks ?? string.Empty,
                    InstituteId = dto.InstituteId
                };
                _context.Attendances.Add(attendance);
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Attendance recorded successfully." });
    }

    // PUT: api/attendance/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAttendance(int id, [FromBody] Attendance model)
    {
        if (id != model.Id)
        {
            return BadRequest("ID mismatch.");
        }

        var existing = await _context.Attendances.FindAsync(id);
        if (existing == null || existing.IsDeleted)
        {
            return NotFound("Attendance record not found.");
        }

        existing.Status = model.Status;
        existing.Remarks = model.Remarks ?? string.Empty;
        existing.Date = model.Date;

        _context.Attendances.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // GET: api/attendance/monthly-report
    [HttpGet("monthly-report")]
    public async Task<IActionResult> GetMonthlyReport(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int instituteId = 1)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var role = ClaimsHelper.GetRole(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var studentsQuery = _context.Students.Where(s => s.InstituteId == instituteId && !s.IsDeleted);

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff != null)
            {
                var assignedBatches = await _context.Batches
                    .Where(b => b.TeacherId == staff.Id && !b.IsDeleted && b.IsActive)
                    .Select(b => b.BatchName)
                    .ToListAsync();

                studentsQuery = studentsQuery.Where(s => assignedBatches.Contains(s.Batch));
            }
            else
            {
                studentsQuery = studentsQuery.Where(s => false);
            }
        }

        var students = await studentsQuery.ToListAsync();
        var studentIds = students.Select(s => s.Id).ToList();

        var attendances = await _context.Attendances
            .Where(x => x.InstituteId == instituteId && x.Date >= startDate && x.Date <= endDate && studentIds.Contains(x.StudentId) && !x.IsDeleted)
            .ToListAsync();

        var report = students.Select(student =>
        {
            var studentAttendance = attendances.Where(a => a.StudentId == student.Id).ToList();
            var total = studentAttendance.Count;
            var present = studentAttendance.Count(a => a.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
            var absent = studentAttendance.Count(a => a.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase));
            var leave = studentAttendance.Count(a => a.Status.Equals("Leave", StringComparison.OrdinalIgnoreCase));
            var late = studentAttendance.Count(a => a.Status.Equals("Late", StringComparison.OrdinalIgnoreCase));

            double rate = total > 0 ? (double)(present + late) / total * 100 : 100.0;

            return new MonthlyReportItem
            {
                StudentId = student.Id,
                StudentName = student.Name,
                Course = student.Course,
                Batch = student.Batch,
                TotalClasses = total,
                Present = present,
                Absent = absent,
                Leave = leave,
                Late = late,
                AttendancePercentage = Math.Round(rate, 2)
            };
        }).ToList();

        return Ok(report);
    }

    // GET: api/attendance/export
    [HttpGet("export")]
    public async Task<IActionResult> ExportAttendance(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int instituteId = 1)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var role = ClaimsHelper.GetRole(User);
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var query = _context.Attendances
            .Where(x => x.InstituteId == instituteId && x.Date >= startDate && x.Date <= endDate && !x.IsDeleted);

        if (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff != null)
            {
                var assignedBatches = await _context.Batches
                    .Where(b => b.TeacherId == staff.Id && !b.IsDeleted && b.IsActive)
                    .Select(b => b.BatchName)
                    .ToListAsync();
                
                var studentIds = await _context.Students
                    .Where(s => assignedBatches.Contains(s.Batch) && !s.IsDeleted)
                    .Select(s => s.Id)
                    .ToListAsync();

                query = query.Where(x => studentIds.Contains(x.StudentId));
            }
            else
            {
                query = query.Where(x => false);
            }
        }

        var attendances = await query
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StudentName)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Date,Student Name,Status,Remarks");

        foreach (var att in attendances)
        {
            csv.AppendLine($"{att.Date:yyyy-MM-dd},{att.StudentName},{att.Status},{att.Remarks.Replace(",", " ")}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"Attendance_Report_{year}_{month}.csv");
    }
}

public class BulkAttendanceDto
{
    public int InstituteId { get; set; } = 1;
    public DateTime Date { get; set; }
    public List<AttendanceRecordDto> Records { get; set; } = new();
}

public class AttendanceRecordDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Status { get; set; } = "Present";
    public string? Remarks { get; set; }
}

public class MonthlyReportItem
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Batch { get; set; } = string.Empty;
    public int TotalClasses { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Leave { get; set; }
    public int Late { get; set; }
    public double AttendancePercentage { get; set; }
}
