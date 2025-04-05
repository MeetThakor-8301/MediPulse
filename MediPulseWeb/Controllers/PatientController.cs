using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediPulseWeb.Models.DTOs;

namespace MediPulseWeb.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PatientController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Doctors()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/doctor");
            if (response.IsSuccessStatusCode)
            {
                var doctors = await response.Content.ReadFromJsonAsync<IEnumerable<DoctorResponse>>();
                return View(doctors);
            }

            TempData["Error"] = "Failed to retrieve doctors.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DoctorDetails(Guid doctorId)
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/doctor/{doctorId}");
            if (response.IsSuccessStatusCode)
            {
                var doctor = await response.Content.ReadFromJsonAsync<DoctorResponse>();
                return View(doctor);
            }

            TempData["Error"] = "Failed to retrieve doctor details.";
            return RedirectToAction(nameof(Doctors));
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(AppointmentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate.Date,
                StartTime = model.StartTime,
                FinishTime = model.FinishTime,
                Reason = model.Reason
            };

            var response = await client.PostAsJsonAsync("api/patient/appointments", request);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Appointment requested successfully.";
                return RedirectToAction(nameof(Appointments));
            }

            ModelState.AddModelError(string.Empty, "Failed to book appointment.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BookAppointment(Guid doctorId)
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/doctor/{doctorId}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to retrieve doctor details.";
                return RedirectToAction("Doctors");
            }

            var doctor = await response.Content.ReadFromJsonAsync<DoctorResponse>();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor not found.";
                return RedirectToAction("Doctors");
            }

            ViewBag.DoctorId = doctor.Id;
            ViewBag.DoctorName = doctor.FullName;
            ViewBag.DoctorSpecialty = doctor.Speciality;

            var model = new AppointmentRequest
            {
                DoctorId = doctorId,
                AppointmentDate = DateTime.Today.AddDays(1)
            };

            return View(model);
        }

        public async Task<IActionResult> Appointments()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await client.GetAsync($"api/patient/{userId}/appointments");

            if (response.IsSuccessStatusCode)
            {
                var appointments = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentResponse>>();
                return View(appointments);
            }

            TempData["Error"] = "Failed to retrieve appointments.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await client.GetAsync($"api/patient/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var profile = await response.Content.ReadFromJsonAsync<PatientResponse>();
                return View(profile);
            }

            TempData["Error"] = "Failed to retrieve profile.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdatePatientProfileRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await client.PutAsJsonAsync($"api/patient/{userId}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }

            ModelState.AddModelError(string.Empty, "Failed to update profile.");
            return View(model);
        }
    }
} 