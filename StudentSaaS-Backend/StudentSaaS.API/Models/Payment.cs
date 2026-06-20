using System;

namespace StudentSaaS.API.Models;

public class Payment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public decimal TotalFee { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal BalanceAmount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public string PaymentMethod { get; set; } = "Cash"; // Cash, UPI, Bank Transfer, Card

    public string Remarks { get; set; } = string.Empty;

    public int InstituteId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
