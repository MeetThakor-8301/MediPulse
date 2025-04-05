using System.ComponentModel.DataAnnotations;

namespace MediPulse.DTOs;

public record DoctorResponse(
    Guid Id,
    string FullName,
    string Email,
    string Speciality,
    string CertificateUrl,
    bool IsApproved,
    DateTime CreatedAt
);

public record PatientResponse(
    Guid Id,
    string FullName,
    string Email,
    DateTime? DateOfBirth,
    string? Gender,
    string? Address,
    string? Phone,
    DateTime CreatedAt
);

public record UserResponse(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    DateTime CreatedAt
);

public record DoctorStatusResponse(
    bool IsApproved,
    string Speciality,
    string CertificateUrl
); 