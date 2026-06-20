using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Data;
using StudentSaaS.API.Helpers;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int instituteId = 1)
    {
        var today = DateTime.UtcNow.Date;
        var role = ClaimsHelper.GetRole(User);
        var userId = ClaimsHelper.GetUserId(User);
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        // Check if teacher
        bool isTeacher = role.Equals("Teacher", StringComparison.OrdinalIgnoreCase);
        int? teacherStaffId = null;
        List<string> assignedBatchNames = new();

        if (isTeacher)
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Email == email && !s.IsDeleted);
            if (staff != null)
            {
                teacherStaffId = staff.Id;
                assignedBatchNames = await _context.Batches
                    .Where(b => b.TeacherId == staff.Id && !b.IsDeleted && b.IsActive)
                    .Select(b => b.BatchName)
                    .ToListAsync();
            }
        }

        // 1. Total Students
        var studentQuery = _context.Students.Where(s => s.InstituteId == instituteId && !s.IsDeleted);
        if (isTeacher)
        {
            studentQuery = studentQuery.Where(s => assignedBatchNames.Contains(s.Batch));
        }
        var totalStudents = await studentQuery.CountAsync();

        // 2. Present/Absent Today
        var attendanceQuery = _context.Attendances.Where(a => a.InstituteId == instituteId && a.Date.Date == today && !a.IsDeleted);
        if (isTeacher)
        {
            var studentIds = await studentQuery.Select(s => s.Id).ToListAsync();
            attendanceQuery = attendanceQuery.Where(a => studentIds.Contains(a.StudentId));
        }
        var presentToday = await attendanceQuery.CountAsync(a => a.Status == "Present");
        var absentToday = await attendanceQuery.CountAsync(a => a.Status == "Absent");

        // 3. Staff count (Teachers)
        var totalTeachers = await _context.Staff.CountAsync(s => s.InstituteId == instituteId && s.Role == "Teacher" && s.ActiveStatus && !s.IsDeleted);

        // 4. Financial Summary (Payments & Pending Fees)
        var paymentQuery = _context.Payments.Where(p => p.InstituteId == instituteId && !p.IsDeleted);
        if (isTeacher)
        {
            var studentNames = await studentQuery.Select(s => s.Name).ToListAsync();
            paymentQuery = paymentQuery.Where(p => studentNames.Contains(p.StudentName));
        }
        var payments = await paymentQuery.ToListAsync();
        var totalPayments = payments.Count;
        var pendingPaymentsCount = payments.Count(p => p.BalanceAmount > 0);
        var pendingFeesAmount = payments.Sum(p => p.BalanceAmount);
        
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        var monthlyRevenue = payments
            .Where(p => p.PaymentDate.Month == currentMonth && p.PaymentDate.Year == currentYear)
            .Sum(p => p.PaidAmount);

        var totalRevenue = payments.Sum(p => p.PaidAmount);

        // 5. Attendance Trend (Last 7 days)
        var sevenDaysAgo = today.AddDays(-7);
        var trendAttendanceQuery = _context.Attendances
            .Where(a => a.InstituteId == instituteId && a.Date >= sevenDaysAgo && a.Date <= DateTime.UtcNow && !a.IsDeleted);
        if (isTeacher)
        {
            var studentIds = await studentQuery.Select(s => s.Id).ToListAsync();
            trendAttendanceQuery = trendAttendanceQuery.Where(a => studentIds.Contains(a.StudentId));
        }
        var attendanceList = await trendAttendanceQuery.ToListAsync();

        var attendanceTrend = attendanceList
            .GroupBy(a => a.Date.Date)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Present = g.Count(a => a.Status == "Present"),
                Absent = g.Count(a => a.Status == "Absent"),
                Leave = g.Count(a => a.Status == "Leave"),
                Late = g.Count(a => a.Status == "Late")
            })
            .ToList();

        // 6. Revenue Trend (Last 6 months)
        var sixMonthsAgo = new DateTime(currentYear, currentMonth, 1).AddMonths(-5);
        var revenueTrend = payments
            .Where(p => p.PaymentDate >= sixMonthsAgo)
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Month = $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMM yyyy}",
                Revenue = g.Sum(p => p.PaidAmount)
            })
            .ToList();

        // 7. Student Growth Trend (Last 6 months)
        var growthTrend = await studentQuery
            .Where(s => s.AdmissionDate >= sixMonthsAgo)
            .ToListAsync();

        var studentGrowth = growthTrend
            .GroupBy(s => new { s.AdmissionDate.Year, s.AdmissionDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Month = $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMM yyyy}",
                Count = g.Count()
            })
            .ToList();

        // 8. Upcoming Events
        var upcomingEvents = await _context.Events
            .Where(e => e.InstituteId == instituteId && e.EventDate >= today && !e.IsDeleted)
            .OrderBy(e => e.EventDate)
            .Take(5)
            .ToListAsync();

        // 9. Recent Activities
        var recentAdmissions = await studentQuery
            .OrderByDescending(s => s.AdmissionDate)
            .Take(5)
            .Select(s => new RecentActivityDto
            {
                Type = "Admission",
                Title = $"New admission: {s.Name}",
                Description = $"Enrolled in {s.Course} ({s.Batch})",
                Timestamp = s.AdmissionDate
            })
            .ToListAsync();

        var recentPaymentsList = payments
            .OrderByDescending(p => p.PaymentDate)
            .Take(5)
            .Select(p => new RecentActivityDto
            {
                Type = "Payment",
                Title = $"Payment received: {p.StudentName}",
                Description = $"Paid {p.PaidAmount:C} for {p.CourseName} via {p.PaymentMethod}",
                Timestamp = p.PaymentDate
            })
            .ToList();

        var recentActivities = recentAdmissions
            .Concat(recentPaymentsList)
            .OrderByDescending(a => a.Timestamp)
            .Take(5)
            .ToList();

        // 10. User Notifications
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.InstituteId == instituteId && !n.IsRead && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        return Ok(new
        {
            totalStudents,
            presentToday,
            absentToday,
            totalTeachers,
            totalPayments,
            pendingPayments = pendingPaymentsCount,
            pendingFeesAmount,
            monthlyRevenue,
            totalRevenue,
            attendanceTrend,
            revenueTrend,
            studentGrowth,
            upcomingEvents,
            recentActivities,
            notifications
        });
    }
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // Admission, Payment, Attendance
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}