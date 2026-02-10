using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using XcelerateLinks.Mvc.Models.ViewModels; // ensure this matches your actual namespace

namespace XcelerateLinks.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var vm = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(vm);
        }
    }
}