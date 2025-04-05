using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MediPulse.Data;
using MediPulse.DTOs;
using MediPulse.Models;

namespace MediPulse.Controllers;

/// <summary>
/// Controller for doctor operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the approval status of the current doctor
    /// </summary>
    /// <returns>The doctor's approval status and details</returns>
    [HttpGet("status")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(DoctorStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorStatusResponse>> GetStatus()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        
        var doctorDetail = await _context.DoctorDetails
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctorDetail == null)
        {
            return NotFound(new { message = "Doctor details not found" });
        }

        return new DoctorStatusResponse(
            doctorDetail.IsApproved,
            doctorDetail.Speciality,
            doctorDetail.CertificateUrl
        );
    }

    /// <summary>
    /// Gets a list of all approved doctors
    /// </summary>
    /// <returns>A list of approved doctors</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DoctorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetAllApprovedDoctors()
    {
        var doctors = await _context.Users
            .Where(u => u.Role == "Doctor")
            .Include(u => u.DoctorDetail)
            .Where(u => u.DoctorDetail!.IsApproved)
            .Select(u => new DoctorResponse(
                u.Id,
                u.FullName,
                u.Email,
                u.DoctorDetail!.Speciality,
                u.DoctorDetail.CertificateUrl,
                u.DoctorDetail.IsApproved,
                u.CreatedAt
            ))
            .ToListAsync();

        return doctors;
    }

    /// <summary>
    /// Gets details of a specific doctor
    /// </summary>
    /// <param name="doctorId">The ID of the doctor</param>
    /// <returns>The doctor's details</returns>
    [HttpGet("{doctorId}")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorResponse>> GetDoctorDetails(Guid doctorId)
    {
        var doctor = await _context.Users
            .Where(u => u.Role == "Doctor" && u.Id == doctorId)
            .Include(u => u.DoctorDetail)
            .Select(u => new DoctorResponse(
                u.Id,
                u.FullName,
                u.Email,
                u.DoctorDetail!.Speciality,
                u.DoctorDetail.CertificateUrl,
                u.DoctorDetail.IsApproved,
                u.CreatedAt
            ))
            .FirstOrDefaultAsync();

        if (doctor == null)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        return doctor;
    }

    /// <summary>
    /// Gets appointments for a specific doctor
    /// </summary>
    /// <param name="doctorId">The ID of the doctor</param>
    /// <returns>A list of appointments for the doctor</returns>
    [HttpGet("{doctorId}/appointments")]
    [Authorize(Roles = "Doctor,SuperAdmin")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<AppointmentListResponse>>> GetDoctorAppointments(Guid doctorId)
    {
        try
        {
            // Check if the current user is the doctor or a super admin
            var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "SuperAdmin" && currentUserId != doctorId)
            {
                return Forbid();
            }

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentListResponse(
                    a.Id,
                    a.Patient.FullName,
                    a.Doctor.FullName,
                    a.AppointmentDate,
                    a.StartTime,
                    a.FinishTime,
                    a.Status,
                    a.CreatedAt
                ))
                .ToListAsync();

            return appointments;
        }catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            List<AppointmentListResponse> m = new List<AppointmentListResponse>();
            return m;
        }
    }

    /// <summary>
    /// Updates the status of an appointment
    /// </summary>
    /// <param name="appointmentId">The ID of the appointment</param>
    /// <param name="request">The status update request</param>
    /// <returns>The updated appointment</returns>
    [HttpPut("appointments/{appointmentId}/status")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> UpdateAppointmentStatus(Guid appointmentId, UpdateAppointmentStatusRequest request)
    {
        try
        {
            var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == currentUserId);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            if (appointment.DoctorId != currentUserId)
            {
                return Forbid();
            }

            appointment.Status = request.Status;
            await _context.SaveChangesAsync();

            return new AppointmentResponse(
                appointment.Id,
                appointment.PatientId,
                appointment.Patient.FullName,
                appointment.DoctorId,
                appointment.Doctor.FullName,
                appointment.AppointmentDate,
                appointment.StartTime,
                appointment.FinishTime,
                appointment.Status,
                appointment.CreatedAt
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the appointment status." });
        }
    }
} 