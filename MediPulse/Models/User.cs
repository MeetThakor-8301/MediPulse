using System.ComponentModel.DataAnnotations;

namespace MediPulse.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(255)]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public DoctorDetail? DoctorDetail { get; set; }
    public PatientDetail? PatientDetail { get; set; }
} 