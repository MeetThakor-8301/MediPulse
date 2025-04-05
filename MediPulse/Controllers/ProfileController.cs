using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MediPulse.Data;
using MediPulse.DTOs;
using MediPulse.Models;

namespace MediPulse.Controllers;

/// <summary>
/// Controller for user profile operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the current user's profile
    /// </summary>
    /// <returns>The user's profile information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        
        var user = await _context.Users
            .Include(u => u.DoctorDetail)
            .Include(u => u.PatientDetail)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        object? additionalDetails = null;
        
        if (user.Role == "Doctor" && user.DoctorDetail != null)
        {
            additionalDetails = new
            {
                Speciality = user.DoctorDetail.Speciality,
                CertificateUrl = user.DoctorDetail.CertificateUrl,
                IsApproved = user.DoctorDetail.IsApproved
            };
        }
        else if (user.Role == "Patient" && user.PatientDetail != null)
        {
            additionalDetails = new
            {
                DateOfBirth = user.PatientDetail.DateOfBirth,
                Gender = user.PatientDetail.Gender,
                Address = user.PatientDetail.Address,
                Phone = user.PatientDetail.Phone
            };
        }

        return new UserProfileResponse(
            Id: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role,
            CreatedAt: user.CreatedAt,
            AdditionalDetails: additionalDetails
        );
    }

    /// <summary>
    /// Updates the current user's basic profile information
    /// </summary>
    /// <param name="request">The profile update request</param>
    /// <returns>The updated user profile</returns>
    [HttpPut]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> UpdateProfile(UpdateProfileRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        
        var user = await _context.Users
            .Include(u => u.DoctorDetail)
            .Include(u => u.PatientDetail)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.FullName = request.FullName;
        await _context.SaveChangesAsync();

        object? additionalDetails = null;
        
        if (user.Role == "Doctor" && user.DoctorDetail != null)
        {
            additionalDetails = new
            {
                Speciality = user.DoctorDetail.Speciality,
                CertificateUrl = user.DoctorDetail.CertificateUrl,
                IsApproved = user.DoctorDetail.IsApproved
            };
        }
        else if (user.Role == "Patient" && user.PatientDetail != null)
        {
            additionalDetails = new
            {
                DateOfBirth = user.PatientDetail.DateOfBirth,
                Gender = user.PatientDetail.Gender,
                Address = user.PatientDetail.Address,
                Phone = user.PatientDetail.Phone
            };
        }

        return new UserProfileResponse(
            Id: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role,
            CreatedAt: user.CreatedAt,
            AdditionalDetails: additionalDetails
        );
    }

    /// <summary>
    /// Updates the current doctor's profile information
    /// </summary>
    /// <param name="request">The doctor profile update request</param>
    /// <returns>The updated doctor profile</returns>
    [HttpPut("doctor")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> UpdateDoctorProfile(UpdateDoctorProfileRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        
        var user = await _context.Users
            .Include(u => u.DoctorDetail)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.DoctorDetail == null)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        user.FullName = request.FullName;
        user.DoctorDetail.Speciality = request.Speciality;
        user.DoctorDetail.CertificateUrl = request.CertificateUrl;
        
        await _context.SaveChangesAsync();

        var additionalDetails = new
        {
            Speciality = user.DoctorDetail.Speciality,
            CertificateUrl = user.DoctorDetail.CertificateUrl,
            IsApproved = user.DoctorDetail.IsApproved
        };

        return new UserProfileResponse(
            Id: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role,
            CreatedAt: user.CreatedAt,
            AdditionalDetails: additionalDetails
        );
    }

    /// <summary>
    /// Updates the current patient's profile information
    /// </summary>
    /// <param name="request">The patient profile update request</param>
    /// <returns>The updated patient profile</returns>
    [HttpPut("patient")]
    [Authorize(Roles = "Patient")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> UpdatePatientProfile(UpdatePatientProfileRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        
        var user = await _context.Users
            .Include(u => u.PatientDetail)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.PatientDetail == null)
        {
            return NotFound(new { message = "Patient not found" });
        }

        user.FullName = request.FullName;
        user.PatientDetail.DateOfBirth = request.DateOfBirth;
        user.PatientDetail.Gender = request.Gender;
        user.PatientDetail.Address = request.Address;
        user.PatientDetail.Phone = request.Phone;
        
        await _context.SaveChangesAsync();

        var additionalDetails = new
        {
            DateOfBirth = user.PatientDetail.DateOfBirth,
            Gender = user.PatientDetail.Gender,
            Address = user.PatientDetail.Address,
            Phone = user.PatientDetail.Phone
        };

        return new UserProfileResponse(
            Id: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role,
            CreatedAt: user.CreatedAt,
            AdditionalDetails: additionalDetails
        );
    }

    /// <summary>
    /// Changes the current user's password
    /// </summary>
    /// <param name="request">The password change request</param>
    /// <returns>A success message</returns>
    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return BadRequest(new { message = "Current password is incorrect" });
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password changed successfully" });
    }
} 