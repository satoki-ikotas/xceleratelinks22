using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIPSI16.Models;
using XcelerateLinks.Models.ViewModels;

namespace XcelerateLinks.Mvc.Controllers
{
    public class ApplicationsController : ApiControllerBase
    {
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(IHttpClientFactory httpFactory, ILogger<ApplicationsController> logger)
            : base(httpFactory)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync("api/application");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = await SafeReadStringAsync(resp) ?? "Unable to load applications.";
                return View(Array.Empty<Application>());
            }

            var applications = await resp.Content.ReadFromJsonAsync<IEnumerable<Application>>() ?? Array.Empty<Application>();
            if (User.IsInRole("0"))
            {
                return View("IndexAdmin", applications);
            }

            return View(applications);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync($"api/application/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var application = await resp.Content.ReadFromJsonAsync<Application>();
            if (application == null) return RedirectToAction(nameof(Index));
            return View(application);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? opportunityId = null)
        {
            var model = new ApplicationCreateViewModel
            {
                Application = new Application { OpportunityId = opportunityId }
            };
            model.Opportunities = await LoadOpportunitiesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Opportunities = await LoadOpportunitiesAsync();
                return View(model);
            }

            model.Application.UserId ??= GetCurrentUserId();

            var client = CreateAuthorizedClient();
            var resp = await client.PostAsJsonAsync("api/application", model.Application);
            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await SafeReadStringAsync(resp) ?? "Unable to create application.");
                model.Opportunities = await LoadOpportunitiesAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync($"api/application/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var application = await resp.Content.ReadFromJsonAsync<Application>();
            if (application == null) return RedirectToAction(nameof(Index));
            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = CreateAuthorizedClient();
            var resp = await client.DeleteAsync($"api/application/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<Opportunity>> LoadOpportunitiesAsync()
        {
            var client = CreateAuthorizedClient();
            var resp = await client.GetAsync("api/opportunities");
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Unable to load opportunities for application form: {Status}", resp.StatusCode);
                return Array.Empty<Opportunity>();
            }

            return await resp.Content.ReadFromJsonAsync<IEnumerable<Opportunity>>() ?? Array.Empty<Opportunity>();
        }
    }
}
