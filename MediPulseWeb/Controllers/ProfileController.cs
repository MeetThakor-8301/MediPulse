using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediPulseWeb.Models.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MediPulseWeb.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/profile");
            if (response.IsSuccessStatusCode)
            {
                var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
                return View(profile);
            }

            TempData["Error"] = "Failed to retrieve profile.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBasicProfile(UpdateBasicProfileRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync("api/profile", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Failed to update profile.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var token = User.FindFirst("Token")?.Value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync("api/profile/change-password", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Password changed successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Failed to change password.");
            return View(model);
        }
    }
} 