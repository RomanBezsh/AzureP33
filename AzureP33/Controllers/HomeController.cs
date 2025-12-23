using AzureP33.Models;
using AzureP33.Models.Home;
using AzureP33.Models.Orm;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace AzureP33.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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

            var resp = JsonSerializer.Deserialize<LanguagesResponce>(
                await client.GetStringAsync(
                    @"https://api.translator.azure.cn/languages?api-version=3.0"
                )
            );
            if (resp == null )
            {
                throw new Exception("LanguagesResponce got NULL result");
            }
            HomeIndexViewModel viewModel = new()
            {
                PageTitle = "Перекладач",
                FormModel = formModel?.Action == null ? null : formModel,
                LanguagesResponce = resp
            };
            if (formModel?.Action == "translate")
            {
                var sec = _configuration.GetSection("Azure").GetSection("Translator");
                if (!sec.Exists())
                {
                    throw new InvalidOperationException("Missing configuration section: Azure:Translator");
                }

                string key = sec.GetValue<string>("Key") ?? throw new InvalidOperationException("Missing Key");
                string endpoint = sec.GetValue<string>("Endpoint") ?? throw new InvalidOperationException("Missing Endpoint");
                string location = sec.GetValue<string>("Location") ?? throw new InvalidOperationException("Missing Location");
                string translatorPath = sec.GetValue<string>("TranslatorPath") ?? "/translate";
                string apiVersion = sec.GetValue<string>("ApiVersion") ?? "3.0";

                string route = $"{translatorPath}?api-version={apiVersion}&from={formModel.LangFrom}&to={formModel.LangTo}";
                string textToTranslate = formModel.OriginalText;
                object[] body = new object[] { new { Text = textToTranslate } };
                var requestBody = JsonSerializer.Serialize(body);

                using (var client2 = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    // Build the request.
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(endpoint + route);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                    // location required if you're using a multi-service or regional (not global) resource.
                    request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                    // Send the request and get response.
                    HttpResponseMessage response = await client2.SendAsync(request).ConfigureAwait(false);
                    // Read response as a string.
                    string result = await response.Content.ReadAsStringAsync();
                    if (result[0] == '[')
                    {
                        viewModel.Items = JsonSerializer.Deserialize<List<TranslatorResponceItem>>(result);
                    }
                    else
                    {
                        viewModel.ErrorResponce = JsonSerializer.Deserialize<TranslatorErrorResponce>(result);
                    }
                    ViewData["result"] = result; // [{ "translations":[{ "text":"Greetings","to":"en"}]}]
                }


            }

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