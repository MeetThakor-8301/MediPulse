using System.ComponentModel.DataAnnotations;

namespace MediPulseWeb.Models.DTOs
{
    public class AppointmentRequest
    {
        [Required]
        public Guid DoctorId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan FinishTime { get; set; }
        
        [StringLength(500)]
        public string? Reason { get; set; }
    }

    public class UpdateAppointmentStatusRequest
    {
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string DoctorSpecialty { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan FinishTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AppointmentListResponse
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan FinishTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
} 