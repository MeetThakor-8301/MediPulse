using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediPulse.Data;
using MediPulse.DTOs;
using MediPulse.Models;

namespace MediPulse.Controllers;

/// <summary>
/// Controller for admin operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all doctors in the system
    /// </summary>
    /// <returns>A list of all doctors with their details</returns>
    [HttpGet("doctors")]
    [ProducesResponseType(typeof(IEnumerable<DoctorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetAllDoctors()
    {
        try
        {
            var doctors = await _context.Users
                .Where(u => u.Role == "Doctor")
                .Include(u => u.DoctorDetail)
                .AsNoTracking()
                .OrderBy(u => u.CreatedAt)
                .Select(u => new DoctorResponse
                (
                    u.Id,
                    u.FullName ?? "N/A",
                    u.Email ?? "N/A",
                    u.DoctorDetail != null ? u.DoctorDetail.Speciality : "Not Specified",
                    u.DoctorDetail != null ? u.DoctorDetail.CertificateUrl : "Not Available",
                    u.DoctorDetail != null && u.DoctorDetail.IsApproved,
                    u.CreatedAt
                ))
                .ToListAsync();

            return Ok(doctors);
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error fetching doctors: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving doctors.");
        }
    }


    /// <summary>
    /// Gets all patients in the system
    /// </summary>
    /// <returns>A list of all patients with their details</returns>
    [HttpGet("patients")]
    [ProducesResponseType(typeof(IEnumerable<PatientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PatientResponse>>> GetAllPatients()
    {
        try
        {
            var patients = await _context.Users
                .Where(u => u.Role == "Patient")
                .Include(u => u.PatientDetail)
                .AsNoTracking()
                .OrderBy(u => u.CreatedAt)
                .Select(u => new PatientResponse
                (
                    u.Id,
                    u.FullName ?? "N/A",
                    u.Email ?? "N/A",
                    u.PatientDetail != null ? u.PatientDetail.DateOfBirth : null,
                    u.PatientDetail != null ? u.PatientDetail.Gender : "Not Specified",
                    u.PatientDetail != null ? u.PatientDetail.Address : "Not Provided",
                    u.PatientDetail != null ? u.PatientDetail.Phone : "Not Available",
                    u.CreatedAt
                ))
                .ToListAsync();

            return Ok(patients);
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error fetching patients: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving patients.");
        }
    }


    /// <summary>
    /// Gets all users in the system
    /// </summary>
    /// <returns>A list of all users with their basic details</returns>
    [HttpGet("users")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
    {
        var users = await _context.Users
             .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .Select(u => new UserResponse
            (
               u.Id,
                u.FullName ?? "N/A",
                u.Email ?? "N/A",
                u.Role,
                u.CreatedAt
            )).ToListAsync();

        return users;
    }

    /// <summary>
    /// Approves a doctor
    /// </summary>
    /// <param name="id">The ID of the doctor to approve</param>
    /// <returns>A success message</returns>
    [HttpPut("doctors/{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveDoctor(Guid id)
    {
        var doctorDetail = await _context.DoctorDetails
            .FirstOrDefaultAsync(d => d.UserId == id);

        if (doctorDetail == null)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        doctorDetail.IsApproved = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Doctor approved successfully" });
    }

    /// <summary>
    /// Rejects a doctor
    /// </summary>
    /// <param name="id">The ID of the doctor to reject</param>
    /// <returns>A success message</returns>
    [HttpPut("doctors/{id}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectDoctor(Guid id)
    {
        var doctorDetail = await _context.DoctorDetails
            .FirstOrDefaultAsync(d => d.UserId == id);

        if (doctorDetail == null)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        doctorDetail.IsApproved = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Doctor rejected successfully" });
    }
}