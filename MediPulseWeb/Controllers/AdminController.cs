using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediPulseWeb.Models.DTOs;

namespace MediPulseWeb.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Doctors()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/admin/doctors");
            if (response.IsSuccessStatusCode)
            {
                var doctors = await response.Content.ReadFromJsonAsync<IEnumerable<DoctorResponse>>();
                return View(doctors);
            }

            TempData["Error"] = "Failed to retrieve doctors.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Patients()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/admin/patients");
            if (response.IsSuccessStatusCode)
            {
                var patients = await response.Content.ReadFromJsonAsync<IEnumerable<PatientResponse>>();
                return View(patients);
            }

            TempData["Error"] = "Failed to retrieve patients.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveDoctor(Guid id)
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"api/admin/doctors/{id}/approve", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Doctor approved successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to approve doctor.";
            }

            return RedirectToAction(nameof(Doctors));
        }

        [HttpPost]
        public async Task<IActionResult> RejectDoctor(Guid id)
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"api/admin/doctors/{id}/reject", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Doctor rejected successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to reject doctor.";
            }

            return RedirectToAction(nameof(Doctors));
        }

        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/admin/users");
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserResponse>>();
                return View(users);
            }

            TempData["Error"] = "Failed to retrieve users.";
            return RedirectToAction("Index", "Home");
        }
    }
} 