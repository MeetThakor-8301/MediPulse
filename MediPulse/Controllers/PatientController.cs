using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MediPulse.Data;
using MediPulse.DTOs;
using MediPulse.Models;
using System.Numerics;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace MediPulse.Controllers;

/// <summary>
/// Controller for patient operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets details of a specific patient
    /// </summary>
    /// <param name="patientId">The ID of the patient</param>
    /// <returns>The patient's details</returns>
    [HttpGet("{patientId}")]
    [Authorize(Roles = "Patient,SuperAdmin")]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientResponse>> GetPatientDetails(Guid patientId)
    {
        // Check if the current user is the patient or a super admin
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole != "SuperAdmin" && currentUserId != patientId)
        {
            return Forbid();
        }

        var patient = await _context.Users
            .Where(u => u.Role == "Patient" && u.Id == patientId)
            .Include(u => u.PatientDetail)
            .Select(u => new PatientResponse(
                u.Id,
                u.FullName,
                u.Email,
                u.PatientDetail!.DateOfBirth,
                u.PatientDetail.Gender,
                u.PatientDetail.Address,
                u.PatientDetail.Phone,
                u.CreatedAt
            ))
            .FirstOrDefaultAsync();

        if (patient == null)
        {
            return NotFound(new { message = "Patient not found" });
        }

        return patient;
    }

    /// <summary>
    /// Gets appointments for a specific patient
    /// </summary>
    /// <param name="patientId">The ID of the patient</param>
    /// <returns>A list of appointments for the patient</returns>
    [HttpGet("{patientId}/appointments")]
    [Authorize(Roles = "Patient,SuperAdmin")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<AppointmentListResponse>>> GetPatientAppointments(Guid patientId)
    {
        // Check if the current user is the patient or a super admin
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole != "SuperAdmin" && currentUserId != patientId)
        {
            return Forbid();
        }

        var appointments = await _context.Appointments
            .Where(a => a.PatientId == patientId)
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
    }

    /// <summary>
    /// Creates a new appointment for a patient
    /// </summary>
    /// <param name="request">The appointment request</param>
    /// <returns>The created appointment</returns>
    [HttpPost("appointments")]
    [Authorize(Roles = "Patient")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> CreateAppointment(CreateAppointmentRequest request)
    {
        try
        {
            var patientId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));

            // Check if the doctor exists and is approved
            var doctor = await _context.Users
                .Where(u => u.Role == "Doctor" && u.Id == request.DoctorId)
                .Include(u => u.DoctorDetail)
                .FirstOrDefaultAsync();

            if (doctor == null || doctor.DoctorDetail == null || !doctor.DoctorDetail.IsApproved)
            {
                return NotFound(new { message = "Doctor not found or not approved" });
            }

            // Create the appointment
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                DoctorId = request.DoctorId,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                FinishTime = request.FinishTime,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Get the patient details
            var patient = await _context.Users
                .Where(u => u.Id == patientId)
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound(new { message = "Patient not found" });
            }

            return new AppointmentResponse(
                appointment.Id,
                patientId,
                patient.FullName,
                doctor.Id,
                doctor.FullName,
                appointment.AppointmentDate,
                appointment.StartTime,
                appointment.FinishTime,
                appointment.Status,
                appointment.CreatedAt
            );
        }catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new AppointmentResponse(
             new Guid(),
             new Guid(),
             "Meet",
             new Guid(),
             "Meet",
             DateTime.Now,
             TimeSpan.MinValue,
             TimeSpan.MinValue,
             "Pending",
             DateTime.Now
            );
        }
    }

    /// <summary>
    /// Updates a patient's details
    /// </summary>
    /// <param name="patientId">The ID of the patient</param>
    /// <param name="request">The patient update request</param>
    /// <returns>The updated patient details</returns>
    [HttpPut("{patientId}")]
    [Authorize(Roles = "Patient,SuperAdmin")]
    [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientResponse>> UpdatePatient(Guid patientId, UpdatePatientProfileRequest request)
    {
        // Check if the current user is the patient or a super admin
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found"));
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (currentUserRole != "SuperAdmin" && currentUserId != patientId)
        {
            return Forbid();
        }

        var user = await _context.Users
            .Include(u => u.PatientDetail)
            .FirstOrDefaultAsync(u => u.Id == patientId && u.Role == "Patient");

        if (user == null || user.PatientDetail == null)
        {
            return NotFound(new { message = "Patient not found" });
        }

        user.FullName = request.FullName;
        user.PatientDetail.DateOfBirth = request.DateOfBirth;
        user.PatientDetail.Gender = request.Gender ?? user.PatientDetail.Gender;
        user.PatientDetail.Address = request.Address ?? user.PatientDetail.Address;
        user.PatientDetail.Phone = request.Phone ?? user.PatientDetail.Phone;

        await _context.SaveChangesAsync();

        return new PatientResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.PatientDetail.DateOfBirth,
            user.PatientDetail.Gender,
            user.PatientDetail.Address,
            user.PatientDetail.Phone,
            user.CreatedAt
        );
    }
} 