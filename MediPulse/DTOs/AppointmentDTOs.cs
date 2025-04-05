using System.ComponentModel.DataAnnotations;

namespace MediPulse.DTOs;

/// <summary>
/// DTO for creating a new appointment
/// </summary>
public record CreateAppointmentRequest(
    [Required] Guid DoctorId,
    [Required] DateTime AppointmentDate,
    [Required] TimeSpan StartTime,
    [Required] TimeSpan FinishTime

);

/// <summary>
/// DTO for updating appointment status
/// </summary>
public record UpdateAppointmentStatusRequest(
    [Required][StringLength(50)] string Status
);

/// <summary>
/// DTO for appointment response
/// </summary>
public record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    DateTime AppointmentDate,
    TimeSpan StartTime,
    TimeSpan FinishTime,
    string Status,
    DateTime CreatedAt
);

/// <summary>
/// DTO for appointment list response
/// </summary>
public record AppointmentListResponse(
    Guid Id,
    string PatientName,
    string DoctorName,
    DateTime AppointmentDate,
    TimeSpan StartTime,
    TimeSpan FinishTime,
    string Status,
    DateTime CreatedAt
); 