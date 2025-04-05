using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediPulseWeb.Models.DTOs;

namespace MediPulseWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var response = await client.PostAsJsonAsync("api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, authResponse.Id),
                        new Claim(ClaimTypes.Email, authResponse.Email),
                        new Claim(ClaimTypes.Name, authResponse.FullName),
                        new Claim(ClaimTypes.Role, authResponse.Role),
                        new Claim("Token", authResponse.Token)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterDoctor()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterDoctor(DoctorRegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            DoctorRegisterRequestAPI apiModel = new DoctorRegisterRequestAPI();
            apiModel.FullName = model.FullName;
            apiModel.Email = model.Email;
            apiModel.Password = model.Password;
            apiModel.Speciality = model.Speciality;
            
            if (model.Certificate != null)
            {
                try
                {
                    // Create certificates directory if it doesn't exist
                    var certificatesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "certificates");
                    Directory.CreateDirectory(certificatesPath);

                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Certificate.FileName)}";
                    var filePath = Path.Combine(certificatesPath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Certificate.CopyToAsync(stream);
                    }

                    // Store relative path in the API model
                    apiModel.CertificateUrl = $"/uploads/certificates/{fileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Certificate", "Failed to upload certificate. Please try again.");
                    return View(model);
                }
            }

            var response = await client.PostAsJsonAsync("api/auth/register/doctor", apiModel);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, authResponse.Id),
                        new Claim(ClaimTypes.Email, authResponse.Email),
                        new Claim(ClaimTypes.Name, authResponse.FullName),
                        new Claim(ClaimTypes.Role, authResponse.Role),
                        new Claim("Token", authResponse.Token)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
            }

            // If registration fails, delete the uploaded certificate
            if (model.Certificate != null && !string.IsNullOrEmpty(apiModel.CertificateUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", apiModel.CertificateUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterPatient()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPatient(PatientRegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MediPulseAPI");
            var response = await client.PostAsJsonAsync("api/auth/register/patient", model);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, authResponse.Id),
                        new Claim(ClaimTypes.Email, authResponse.Email),
                        new Claim(ClaimTypes.Name, authResponse.FullName),
                        new Claim(ClaimTypes.Role, authResponse.Role),
                        new Claim("Token", authResponse.Token)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 