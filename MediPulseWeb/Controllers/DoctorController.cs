using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediPulseWeb.Models.DTOs;

namespace MediPulseWeb.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DoctorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Status()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/doctor/status");
            if (response.IsSuccessStatusCode)
            {
                var status = await response.Content.ReadFromJsonAsync<DoctorStatusResponse>();
                return View(status);
            }

            TempData["Error"] = "Failed to retrieve doctor status.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Appointments()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await client.GetAsync($"api/doctor/{userId}/appointments");
            
            if (response.IsSuccessStatusCode)
            {
                var appointments = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentResponse>>();
                return View(appointments);
            }

            TempData["Error"] = "Failed to retrieve appointments.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(Guid appointmentId, string status)
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new UpdateAppointmentStatusRequest { Status = status };
            var response = await client.PutAsJsonAsync($"api/doctor/appointments/{appointmentId}/status", request);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Appointment status updated successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to update appointment status.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        public async Task<IActionResult> Profile()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await client.GetAsync($"api/doctor/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var profile = await response.Content.ReadFromJsonAsync<DoctorResponse>();
                return View(profile);
            }

            TempData["Error"] = "Failed to retrieve profile.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateDoctorProfileRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.FullName), "FullName");
            formData.Add(new StringContent(model.Speciality), "Speciality");
            
            if (model.Certificate != null)
            {
                var fileContent = new StreamContent(model.Certificate.OpenReadStream());
                formData.Add(fileContent, "Certificate", model.Certificate.FileName);
            }

            var response = await client.PutAsync("api/profile/doctor", formData);

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