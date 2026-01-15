using AzureP33.Models;
using AzureP33.Models.Home;
using AzureP33.Models.Orm;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
namespace AzureP33.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private static LanguagesResponse? _languagesResponse;
        
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> IndexAsync(HomeIndexFormModel? formModel)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState["original-text"]?.Errors.FirstOrDefault();
                if (error != null)
                {
                    ViewBag.OriginalTextError = error.ErrorMessage;
                }
            }
            Task<LanguagesResponse> respTask = GetLanguagesAsync();
            HomeIndexViewModel viewModel = new()
            {
                PageTitle = "Перекладач",
                FormModel = formModel?.Action == null ? null : formModel,
            };
            if (formModel?.Action == "translate")
            {

                string query = $"from={formModel.LangFrom}&to={formModel.LangTo}";
                string textToTranslate = formModel.OriginalText;
                object[] body = new object[] { new { Text = textToTranslate } };
                var requestBody = JsonSerializer.Serialize(body);


                string result = await RequestApi(query, requestBody, ApiMode.Translate);
                if (!string.IsNullOrWhiteSpace(result) && result.TrimStart().StartsWith("["))
                {
                    viewModel.Items = JsonSerializer.Deserialize<List<TranslatorResponseItem>>(result);
                }
                else
                {
                    viewModel.ErrorResponse = JsonSerializer.Deserialize<TranslatorErrorResponse>(result);

                }
                ViewData["result"] = result;

            }
            ;

            if (respTask == null)
            {
                throw new Exception("LanguagesResponce got NULL result");
            }

            var resp = await respTask;

            if (viewModel.Items != null)
            {
                LangData langData;

                try
                {
                    langData = resp.Transliterations[formModel!.LangFrom];
                    string fromScript = langData.Scripts![0].Code!;
                    string toScript = langData.Scripts![0].ToScripts![0].Code!;
                    string query = $"language={formModel.LangFrom}&fromScript={fromScript}&toScript={toScript}";
                    var requestBody = JsonSerializer.Serialize(new object[]
                    {
                        new
                        {
                            Text = formModel.OriginalText
                        }
                    });
                    string result = await RequestApi(query, requestBody, ApiMode.Transliterate);
                    viewModel.FromTransliteration = JsonSerializer.Deserialize<List<TransliteratorResponseItem>>(result)![0];
                }
                catch (Exception ex)
                {
                    ViewData["transliterationResult"] = $"Error: {ex.Message}";
                }
                try
                {
                    langData = resp.Transliterations[formModel!.LangTo];
                    string fromScript = langData.Scripts![0].Code!;
                    string toScript = langData.Scripts![0].ToScripts![0].Code!;
                    string query = $"language={formModel.LangTo}&fromScript={fromScript}&toScript={toScript}";
                    var requestBody = JsonSerializer.Serialize(new object[]
                    {
                        new
                        {
                            Text = viewModel.Items[0].Translations[0].Text
                        }
                    });
                    string result = await RequestApi(query, requestBody, ApiMode.Transliterate);
                    viewModel.ToTransliteration = JsonSerializer.Deserialize<List<TransliteratorResponseItem>>(result)![0];
                }
                catch (Exception ex)
                {
                    ViewData["transliterationResult"] = $"Error: {ex.Message}";
                }
            }


            viewModel.LanguagesResponse = await respTask;
            return View(viewModel);
        }

        public async Task<JsonResult> FetchTranslationAsync(HomeIndexFormModel? formModel)
        {

            return Json(formModel);
        }

        private async Task<LanguagesResponse> GetLanguagesAsync()
        {
            if (_languagesResponse == null)
            {
                using HttpClient client = new();

                _languagesResponse = JsonSerializer.Deserialize<LanguagesResponse>(
                    await client.GetStringAsync(
                        @"https://api.translator.azure.cn/languages?api-version=3.0"
                    )
                );

                ArgumentNullException.ThrowIfNull(_languagesResponse, nameof(_languagesResponse));
            }
            return _languagesResponse;
        }
        private async Task<string> RequestApi(string query, string body, ApiMode apiMode)
        {
            var sec = _configuration.GetSection("Azure").GetSection("Translator") ?? throw new InvalidOperationException("Connection error");

            string key = sec.GetValue<string>("Key") ?? throw new InvalidOperationException("Missing Key");
            string endpoint = sec.GetValue<string>("Endpoint") ?? throw new InvalidOperationException("Missing Endpoint");
            string location = sec.GetValue<string>("Location") ?? throw new InvalidOperationException("Missing Location");
            string apiVersion = sec.GetValue<string>("ApiVersion") ?? "3.0";
            string apiPath = apiMode switch
            {
                ApiMode.Translate => sec.GetValue<string>("TranslatorPath"),
                ApiMode.Transliterate => sec.GetValue<string>("TransliteratorPath"),
                _ => null,
            } ?? throw new Exception("apiMode error");
            using (var client2 = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri($"{endpoint}{apiPath}?api-version={apiVersion}&{query}");
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);
                HttpResponseMessage response = await client2.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                
                return result;
            }
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
    enum ApiMode
    { 
        Translate,
        Transliterate
    }
}