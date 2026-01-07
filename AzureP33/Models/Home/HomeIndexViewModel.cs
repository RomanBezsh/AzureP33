using AzureP33.Models.Orm;

namespace AzureP33.Models.Home
{
    public class HomeIndexViewModel
    {
        public string PageTitle { get; set; } = null!;
        public HomeIndexFormModel? FormModel { get; set; }
        public LanguagesResponse? LanguagesResponse { get; set; } = null!;
        public TranslatorErrorResponse? ErrorResponse { get; set; }
        public List<TranslatorResponseItem>? Items { get; set; }

    }
}

