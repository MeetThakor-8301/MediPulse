using System.ComponentModel.DataAnnotations;

namespace MediPulse.DTOs;

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record RegisterRequest(
    [Required][StringLength(255)] string FullName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required] string Role
);

public record DoctorRegisterRequest(
    [Required][StringLength(255)] string FullName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required][StringLength(255)] string Speciality,
    [Required] string CertificateUrl
);

public record PatientRegisterRequest(
    [Required][StringLength(255)] string FullName,
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    DateTime? DateOfBirth,
    string Gender,
    string Address,
    string Phone
);

public record AuthResponse(
    string Token,
    Guid Id,
    string Role,
    string FullName,
    string Email
); 