using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PaymentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/payments
    [HttpGet]
    public async Task<IActionResult> GetPayments(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int instituteId = 1)
    {
        var query = _context.Payments.Where(p => p.InstituteId == instituteId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.StudentName.Contains(search) || p.CourseName.Contains(search) || p.Remarks.Contains(search));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.PaymentDate)
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

    // GET: api/payments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            return NotFound("Payment not found");

        return Ok(payment);
    }

    // POST: api/payments
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Fetch student and course details to verify
        var student = await _context.Students.FindAsync(payment.StudentId);
        if (student == null)
            return BadRequest("Invalid StudentId");

        var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseName == student.Course && c.InstituteId == payment.InstituteId);
        
        payment.StudentName = student.Name;
        payment.CourseName = student.Course;
        if (course != null)
        {
            payment.CourseId = course.Id;
        }

        payment.BalanceAmount = payment.TotalFee - payment.PaidAmount;

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
    }

    // PUT: api/payments/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
    {
        if (id != payment.Id)
            return BadRequest("ID mismatch");

        var existing = await _context.Payments.FindAsync(id);
        if (existing == null)
            return NotFound("Payment not found");

        existing.StudentId = payment.StudentId;
        existing.StudentName = payment.StudentName;
        existing.CourseId = payment.CourseId;
        existing.CourseName = payment.CourseName;
        existing.TotalFee = payment.TotalFee;
        existing.PaidAmount = payment.PaidAmount;
        existing.BalanceAmount = payment.TotalFee - payment.PaidAmount;
        existing.PaymentDate = payment.PaymentDate;
        existing.PaymentMethod = payment.PaymentMethod;
        existing.Remarks = payment.Remarks ?? string.Empty;

        _context.Payments.Update(existing);
        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE: api/payments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            return NotFound("Payment not found");

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Payment deleted successfully." });
    }

    // GET: api/payments/pending
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingPayments([FromQuery] int instituteId = 1)
    {
        var students = await _context.Students
            .Where(s => s.InstituteId == instituteId && !s.IsDeleted && s.Status == "Active")
            .ToListAsync();

        var courses = await _context.Courses
            .Where(c => c.InstituteId == instituteId)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => p.InstituteId == instituteId)
            .ToListAsync();

        var pendingList = new List<PendingPaymentDto>();

        foreach (var student in students)
        {
            // Find student's course fee
            var studentCourse = courses.FirstOrDefault(c => c.CourseName.Equals(student.Course, StringComparison.OrdinalIgnoreCase));
            var fee = studentCourse?.Fee ?? 1000m; // Fallback default fee

            // Calculate total paid by student
            var studentPayments = payments.Where(p => p.StudentId == student.Id).ToList();
            var totalPaid = studentPayments.Sum(p => p.PaidAmount);

            // Since it's a SaaS, drawing classes are paid monthly.
            // Let's check how many months the student has been enrolled.
            var monthsEnrolled = Math.Max(1, ((DateTime.UtcNow.Year - student.AdmissionDate.Year) * 12) + DateTime.UtcNow.Month - student.AdmissionDate.Month + 1);
            var expectedTotal = fee * monthsEnrolled;

            var balance = expectedTotal - totalPaid;

            if (balance > 0)
            {
                var latestPaymentDate = studentPayments.OrderByDescending(p => p.PaymentDate).FirstOrDefault()?.PaymentDate;
                var baseDateForOverdue = latestPaymentDate ?? student.AdmissionDate;
                var daysOverdue = (DateTime.UtcNow - baseDateForOverdue).Days;
                
                // Cap days overdue to avoid massive numbers for seeded/old mock students
                if (daysOverdue < 0) daysOverdue = 0;

                pendingList.Add(new PendingPaymentDto
                {
                    StudentId = student.Id,
                    StudentName = student.Name,
                    ParentPhone = student.ParentPhone,
                    CourseName = student.Course,
                    DueAmount = balance,
                    DaysOverdue = daysOverdue,
                    OverdueStatus = daysOverdue > 60 ? "Danger" : (daysOverdue > 30 ? "Warning" : "Info")
                });
            }
        }

        return Ok(pendingList.OrderByDescending(x => x.DaysOverdue).ToList());
    }
}

public class PendingPaymentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public decimal DueAmount { get; set; }
    public int DaysOverdue { get; set; }
    public string OverdueStatus { get; set; } = "Info"; // Info, Warning, Danger
}
