using Microsoft.EntityFrameworkCore;
using MediPulse.Models;

namespace MediPulse.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<DoctorDetail> DoctorDetails { get; set; } = null!;
    public DbSet<PatientDetail> PatientDetails { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion(
                v => v.ToString(),
                v => v.ToString()
            );

        // Configure Appointment relationships
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany()
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed SuperAdmin
        var superAdminId = Guid.NewGuid();
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = superAdminId,
                FullName = "Super Admin",
                Email = "admin@medipulse.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "SuperAdmin",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
} 