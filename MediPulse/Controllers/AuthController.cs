using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediPulse.Data;
using MediPulse.DTOs;
using MediPulse.Models;
using MediPulse.Services;

namespace MediPulse.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">The login credentials</param>
    /// <returns>A JWT token and user details</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(
            Token: token,
            Id: user.Id,
            Role: user.Role,
            FullName: user.FullName,
            Email: user.Email
        );
    }

    /// <summary>
    /// Registers a new doctor
    /// </summary>
    /// <param name="request">The doctor registration details</param>
    /// <returns>A JWT token and user details</returns>
    [HttpPost("register/doctor")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> RegisterDoctor(DoctorRegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Doctor",
            CreatedAt = DateTime.UtcNow
        };

        var doctorDetail = new DoctorDetail
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Speciality = request.Speciality,
            CertificateUrl = request.CertificateUrl,
            IsApproved = false
        };

        _context.Users.Add(user);
        _context.DoctorDetails.Add(doctorDetail);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(
            Token: token,
            Id : user.Id,
            Role: user.Role,
            FullName: user.FullName,
            Email: user.Email
        );
    }

    /// <summary>
    /// Registers a new patient
    /// </summary>
    /// <param name="request">The patient registration details</param>
    /// <returns>A JWT token and user details</returns>
    [HttpPost("register/patient")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> RegisterPatient(PatientRegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Patient",
            CreatedAt = DateTime.UtcNow
        };

        var patientDetail = new PatientDetail
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            Phone = request.Phone
        };

        _context.Users.Add(user);
        _context.PatientDetails.Add(patientDetail);
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(
            Token: token,
            Id : user.Id,
            Role: user.Role,
            FullName: user.FullName,
            Email: user.Email
        );
    }
} 