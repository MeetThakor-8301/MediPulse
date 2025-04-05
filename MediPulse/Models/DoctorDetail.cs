using System.ComponentModel.DataAnnotations;

namespace MediPulse.Models;

public class DoctorDetail
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Speciality { get; set; } = null!;

    [Required]
    public string CertificateUrl { get; set; } = null!;

    public bool IsApproved { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
} 