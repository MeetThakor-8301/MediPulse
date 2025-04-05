using System.ComponentModel.DataAnnotations;

namespace MediPulse.Models;

public class PatientDetail
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Gender { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
} 