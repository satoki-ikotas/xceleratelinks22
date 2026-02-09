using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIPSI16.Models;
using XcelerateLinks.Models.ViewModels;

namespace XcelerateLinks.Mvc.Controllers
{
    public class UsersController : ApiControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(IHttpClientFactory httpFactory, ILogger<UsersController> logger)
            : base(httpFactory)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? jobPreference = null, int? nationality = null)
        {
            if (User.IsInRole("0"))
            {
                var adminModel = await LoadUsersAsync(jobPreference, nationality, allowEmpty: true);
                return View("IndexAdmin", adminModel);
            }

            if (User.IsInRole("2"))
            {
                if (!jobPreference.HasValue && !nationality.HasValue)
                {
                    return View(new UserFilterViewModel
                    {
                        JobPreference = jobPreference,
                        Nationality = nationality,
                        Users = Array.Empty<User>(),
                        ErrorMessage = "Use filtros para encontrar candidatos."
                    });
                }

                var employerModel = await LoadUsersAsync(jobPreference, nationality, allowEmpty: false);
                return View(employerModel);
            }

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync($"api/users/{userId.Value}");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = await SafeReadStringAsync(resp) ?? "Não foi possível carregar o perfil.";
                return View("IndexSelf", new User());
            }

            var user = await resp.Content.ReadFromJsonAsync<User>() ?? new User();
            return View("IndexSelf", user);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync($"api/users/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await resp.Content.ReadFromJsonAsync<User>();
            if (user == null) return RedirectToAction(nameof(Index));
            return View(user);
        }

        public async Task<IActionResult> Profile()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync($"api/users/{userId.Value}");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = await SafeReadStringAsync(resp) ?? "Não foi possível carregar o perfil.";
                return View("IndexSelf", new User());
            }

            var user = await resp.Content.ReadFromJsonAsync<User>() ?? new User();
            return View("IndexSelf", user);
        }

        [HttpGet]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync($"api/users/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await resp.Content.ReadFromJsonAsync<User>();
            if (user == null) return RedirectToAction(nameof(Index));
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = CreateAuthorizedClient();
            var resp = await client.DeleteAsync($"api/users/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<UserFilterViewModel> LoadUsersAsync(int? jobPreference, int? nationality, bool allowEmpty)
        {
            var model = new UserFilterViewModel
            {
                JobPreference = jobPreference,
                Nationality = nationality
            };

            var client = CreateAuthorizedClient();
            var query = new List<string>();
            if (jobPreference.HasValue) query.Add($"jobPreference={jobPreference.Value}");
            if (nationality.HasValue) query.Add($"nationality={nationality.Value}");

            if (query.Count == 0 && !allowEmpty)
            {
                model.Users = Array.Empty<User>();
                model.ErrorMessage = "Use filtros para encontrar candidatos.";
                return model;
            }

            var url = query.Count == 0 ? "api/users" : $"api/users?{string.Join("&", query)}";
            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
            {
                model.ErrorMessage = await SafeReadStringAsync(resp) ?? "Unable to load users with the selected filters.";
                model.Users = Array.Empty<User>();
                return model;
            }

            model.Users = await resp.Content.ReadFromJsonAsync<IEnumerable<User>>() ?? Array.Empty<User>();
            return model;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync($"api/users/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await resp.Content.ReadFromJsonAsync<User>();
            if (user == null) return RedirectToAction(nameof(Index));
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User model)
        {
            if (id != model.UserId) return RedirectToAction(nameof(Index));
            if (!ModelState.IsValid) return View(model);

            var client = CreateAuthorizedClient();
            var resp = await client.PutAsJsonAsync($"api/users/{id}", model);
            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await SafeReadStringAsync(resp) ?? "Unable to update user.");
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
