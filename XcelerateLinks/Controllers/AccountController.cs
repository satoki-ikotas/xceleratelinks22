using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using XcelerateLinks.Models.ViewModels;

namespace XcelerateLinks.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<AccountController> _logger;
        private readonly IWebHostEnvironment _env;
        private const string CookieName = "ApiAccessToken";

        public AccountController(IHttpClientFactory httpFactory, ILogger<AccountController> logger, IWebHostEnvironment env)
        {
            _httpFactory = httpFactory;
            _logger = logger;
            _env = env;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
            => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpFactory.CreateClient("Api");
            var payload = new { username = model.Username?.Trim(), password = model.Password };

            try
            {
                var resp = await client.PostAsJsonAsync("api/auth/login", payload);

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await SafeReadStringAsync(resp);
                    ModelState.AddModelError("", !string.IsNullOrWhiteSpace(body) ? $"Login failed: {body}" : $"Login failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");
                    return View(model);
                }

                var auth = await resp.Content.ReadFromJsonAsync<AuthResponse?>();
                if (auth == null || string.IsNullOrWhiteSpace(auth.Token))
                {
                    ModelState.AddModelError("", "Login failed: invalid response from authentication service.");
                    return View(model);
                }

                Response.Cookies.Append(CookieName, auth.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict,
                    Expires = auth.ExpiresAt.ToUniversalTime()
                });

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Username ?? string.Empty) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login exception");
                ModelState.AddModelError("", "Unable to contact authentication service.");
                if (_env.IsDevelopment()) ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(CookieName);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpFactory.CreateClient("Api");
            var payload = new
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Nationality = model.Nationality,
                JobPreference = model.JobPreference,
                Profile_Bio = model.Profile_Bio,
                DoB = model.DoB,
                Role = model.Role,
                Password = model.Password
            };

            try
            {
                var resp = await client.PostAsJsonAsync("api/auth/register", payload);
                if (!resp.IsSuccessStatusCode)
                {
                    var body = await SafeReadStringAsync(resp);
                    ModelState.AddModelError("", !string.IsNullOrWhiteSpace(body) ? $"Registration failed: {body}" : $"Registration failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");
                    return View(model);
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register exception");
                ModelState.AddModelError("", "Unable to contact authentication service.");
                return View(model);
            }
        }

        // helper to read response body without throwing
        private static async Task<string?> SafeReadStringAsync(HttpResponseMessage resp)
        {
            try
            {
                return resp.Content == null ? null : await resp.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}