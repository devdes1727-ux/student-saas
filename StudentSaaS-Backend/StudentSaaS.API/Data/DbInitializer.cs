using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentSaaS.API.Models;
using StudentSaaS.API.Helpers;

namespace StudentSaaS.API.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is migrated
        await context.Database.MigrateAsync();

        // Seed Roles
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "Teacher" }
            );
            await context.SaveChangesAsync();
        }

        // 1. Seed default Institute
        var institute = await context.Institutes.FirstOrDefaultAsync();
        if (institute == null)
        {
            institute = new Institute
            {
                Name = "Sivaram Kalaikoodam Art Academy",
                Phone = "+91 98765 43210",
                LogoUrl = "",
                ThemeSettings = "dark",
                EmailSettings = "smtp.drawingclass.com",
                WhatsAppNumber = "+91 98765 43210",
                Address = "Tirunelveli, Tamil Nadu, India",
                SocialMediaLinks = "{\"facebook\":\"#\",\"instagram\":\"#\",\"twitter\":\"#\"}",
                IsActive = true
            };
            context.Institutes.Add(institute);
            await context.SaveChangesAsync();
        }

        // 2. Seed Admin User
        var adminEmail = "admin@drawingclass.com";
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                Name = "Sivaram Admin",
                Email = adminEmail,
                PasswordHash = PasswordHelper.Hash("Password@123"),
                Role = "Admin",
                Phone = "+91 98765 43210",
                InstituteId = institute.Id,
                IsActive = true
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        // 3. Seed some default Courses if empty
        if (!context.Courses.Any())
        {
            context.Courses.AddRange(
                new Course { CourseName = "Pencil Drawing", CourseCode = "PD01", Description = "Learn shading, lines and realism", Fee = 1500, DurationMonths = 3, Category = "Pencil", InstituteId = institute.Id, IsActive = true },
                new Course { CourseName = "Watercolor Painting", CourseCode = "WP01", Description = "Learn wet on wet and color harmony", Fee = 2000, DurationMonths = 4, Category = "Watercolor", InstituteId = institute.Id, IsActive = true },
                new Course { CourseName = "Oil Painting", CourseCode = "OP01", Description = "Learn oil canvas techniques", Fee = 3500, DurationMonths = 6, Category = "Oil Painting", InstituteId = institute.Id, IsActive = true }
            );
            await context.SaveChangesAsync();
        }

        // 4. Seed default Testimonials if empty
        if (!context.Testimonials.Any())
        {
            context.Testimonials.AddRange(
                new Testimonial { Name = "Priya R.", Role = "Parent", Message = "Sivaram Kalaikoodam helped my child gain confidence and artistic discipline.", Rating = 5, ImageUrl = "", InstituteId = institute.Id },
                new Testimonial { Name = "Arun K.", Role = "Student", Message = "The workshops and competition training were truly next level.", Rating = 5, ImageUrl = "", InstituteId = institute.Id }
            );
            await context.SaveChangesAsync();
        }

        // 5. Seed default Events if empty
        if (!context.Events.Any())
        {
            context.Events.AddRange(
                new Event { Title = "Summer Exhibition", Description = "Annual display of student artwork", ImageUrl = "", Category = "Exhibition", EventDate = new DateTime(2024, 7, 15), Location = "Tirunelveli", InstituteId = institute.Id },
                new Event { Title = "Kids Drawing Competition", Description = "Drawing competition for kids aged 5 to 15", ImageUrl = "", Category = "Competition", EventDate = new DateTime(2025, 5, 20), Location = "Kalaikoodam Campus", InstituteId = institute.Id },
                new Event { Title = "Annual Art Festival", Description = "Grand annual arts celebration", ImageUrl = "", Category = "Festival", EventDate = new DateTime(2026, 12, 10), Location = "Town Hall", InstituteId = institute.Id }
            );
            await context.SaveChangesAsync();
        }

        // 6. Seed default Gallery if empty
        if (!context.GalleryImages.Any())
        {
            context.GalleryImages.AddRange(
                new GalleryImage { Title = "Award Ceremony", ImageUrl = "https://images.unsplash.com/photo-1544928147-79a2df17a3f0?w=600", Category = "Events", IsFeatured = true, InstituteId = institute.Id },
                new GalleryImage { Title = "Student Sketches", ImageUrl = "https://images.unsplash.com/photo-1579783902614-a3fb3927b6a5?w=600", Category = "Artwork", IsFeatured = true, InstituteId = institute.Id },
                new GalleryImage { Title = "Watercolor Landscape", ImageUrl = "https://images.unsplash.com/photo-1579783928591-7240c663f4b0?w=600", Category = "Watercolor", IsFeatured = false, InstituteId = institute.Id }
            );
            await context.SaveChangesAsync();
        }
    }
}
