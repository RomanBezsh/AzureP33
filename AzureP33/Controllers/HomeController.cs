using AzureP33.Models;
using AzureP33.Models.Home;
using AzureP33.Models.Orm;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace AzureP33.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(HomeIndexFormModel? formModel)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState["original-text"]?.Errors.FirstOrDefault();
                if (error != null)
                {
                    ViewBag.OriginalTextError = error.ErrorMessage;
                }
            }

            using HttpClient client = new();

            var response = JsonSerializer.Deserialize<LanguagesResponce>(
                await client.GetStringAsync(
                    @"https://api.translator.azure.cn/languages?api-version=3.0"
                )
            );
            if (response == null )
            {
                throw new Exception("LanguagesResponce got NULL result");
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