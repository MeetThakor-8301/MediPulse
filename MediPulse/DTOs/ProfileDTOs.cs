using System.ComponentModel.DataAnnotations;

namespace MediPulse.DTOs;

/// <summary>
/// DTO for updating user profile information
/// </summary>
public record UpdateProfileRequest(
    [Required][StringLength(255)] string FullName
);

/// <summary>
/// DTO for updating doctor profile information
/// </summary>
public record UpdateDoctorProfileRequest(
    [Required][StringLength(255)] string FullName,
    [Required][StringLength(255)] string Speciality,
    [Required] string CertificateUrl
);

/// <summary>
/// DTO for updating patient profile information
/// </summary>
public record UpdatePatientProfileRequest(
    [Required][StringLength(255)] string FullName,
    DateTime? DateOfBirth,
    string? Gender,
    string? Address,
    string? Phone
);

/// <summary>
/// DTO for changing user password
/// </summary>
public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required][MinLength(6)] string NewPassword
);

/// <summary>
/// DTO for user profile response
/// </summary>
public record UserProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    DateTime CreatedAt,
    object? AdditionalDetails
); 