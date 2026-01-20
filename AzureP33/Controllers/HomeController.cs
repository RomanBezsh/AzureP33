using AzureP33.Models;
using AzureP33.Models.Cosmos;
using AzureP33.Models.Home;
using AzureP33.Models.Orm;
using AzureP33.Services.CosmosDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Container = Microsoft.Azure.Cosmos.Container;

namespace AzureP33.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICosmosDBService _cosmosDBService;
        private static LanguagesResponse? _languagesResponse;
        
        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            ICosmosDBService cosmosDBService)
        {
            _logger = logger;
            _configuration = configuration;
            _cosmosDBService = cosmosDBService;
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

        [HttpGet]
        public async Task<JsonResult> FetchTranslationAsync(HomeIndexFormModel formModel)
        {
            LanguagesResponse resp = await GetLanguagesAsync();
            if (! resp.Translations.ContainsKey(formModel.LangFrom))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json($"LangFrom '{formModel.LangFrom}' unsupported");
            }

            if (!resp.Translations.ContainsKey(formModel.LangTo))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json($"LangTo '{formModel.LangTo}' unsupported");
            }

            if (formModel.Action != "fetch")
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json($"Action '{formModel.Action}' unsupported");
            }

            if (string.IsNullOrWhiteSpace(formModel.OriginalText))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json($"OriginalText is empty");
            }
             
            try
            {
                return Json(await RequestTranslationAsync(formModel));
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Json($"InternalServerError");
            }



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

        private async Task<string> RequestTranslationAsync(HomeIndexFormModel formModel)
        {
            string query = $"from={formModel.LangFrom}&to={formModel.LangTo}";
            string textToTranslate = formModel.OriginalText;
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonSerializer.Serialize(body);


            string translation = await RequestTranslationAsync(formModel);
            return JsonSerializer.Deserialize<List<TranslatorResponseItem>>(translation)![0].Translations[0].Text;
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

        public async Task<IActionResult> CosmosAsync()
        {

            Container container = await _cosmosDBService.GetConteinerAsync();

            QueryDefinition query = new QueryDefinition(
                query: "SELECT * FROM c WHERE c.categoryId = @category"
            ).WithParameter("@category", "26C74104-40BC-4541-8EF5-9892F7F03D72");

            using FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(
                queryDefinition: query
            );

            List<Product> items = new();
            double requestCharge = 0;
            while (feed.HasMoreResults)
            {
                FeedResponse<Product> response = await feed.ReadNextAsync();
                foreach (Product item in response)
                {
                    items.Add(item);
                }
                requestCharge += response.RequestCharge;
            }

            return View(new HomeCosmosViewModel
            {
                Products = items,
                RequestCharge = requestCharge,
            });
            /*
             * Д.З. Виконати код підключення до БД з домашніх ПК,
             * дослідити можливість встановлення з'єднання та 
             * вибірки даних.
             * Прикласти посилання на сторінку вашого сайту.
             */
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