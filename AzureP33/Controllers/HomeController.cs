using AzureP33.Models;
using AzureP33.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureP33.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(HomeIndexFormModel? formModel)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState["original-text"]?.Errors.FirstOrDefault();
                if (error != null)
                {
                    ViewBag.OriginalTextError = error.ErrorMessage;
                }
            }

            HomeIndexViewModel viewModel = new()
            {
                PageTitle = "Перекладач",
                FormModel = formModel?.Action == null ? null : formModel,
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}