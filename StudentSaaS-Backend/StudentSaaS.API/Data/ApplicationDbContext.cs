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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>()
            .Property(x => x.Fee)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}