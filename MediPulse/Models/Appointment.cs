using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediPulse.Models;

public class Appointment
{
    public Guid Id { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PatientId")]
    public User Patient { get; set; } = null!;

    [ForeignKey("DoctorId")]
    public User Doctor { get; set; } = null!;
} 