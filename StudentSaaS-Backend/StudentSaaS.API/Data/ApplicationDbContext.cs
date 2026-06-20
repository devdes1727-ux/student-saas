using Microsoft.EntityFrameworkCore;
using StudentSaaS.API.Models;

namespace StudentSaaS.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Institute> Institutes => Set<Institute>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<ContactEnquiry> ContactEnquiries => Set<ContactEnquiry>();
    
    // New DbSets matching merged requirements
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<FeePlan> FeePlans => Set<FeePlan>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Enquiry> Enquiries => Set<Enquiry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>()
            .Property(x => x.Fee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(x => x.TotalFee)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(x => x.PaidAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(x => x.BalanceAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<FeePlan>()
            .Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Staff>()
            .Property(x => x.Salary)
            .HasPrecision(18, 2);

        // Map GalleryImage entity to GalleryItems table
        modelBuilder.Entity<GalleryImage>()
            .ToTable("GalleryItems");

        // Map relationships
        modelBuilder.Entity<Student>()
            .HasOne(s => s.CourseRef)
            .WithMany()
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.BatchRef)
            .WithMany()
            .HasForeignKey(s => s.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Batch>()
            .HasOne(b => b.Teacher)
            .WithMany()
            .HasForeignKey(b => b.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Batch>()
            .HasOne(b => b.Course)
            .WithMany()
            .HasForeignKey(b => b.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}