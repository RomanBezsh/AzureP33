using AzureP33.Models.Orm;

namespace AzureP33.Models.Home
{
    public class HomeIndexViewModel
    {
        public string PageTitle { get; set; } = null!;
        public HomeIndexFormModel? FormModel { get; set; }
        public LanguagesResponce? LanguagesResponce { get; set; } = null!;
        public TranslatorErrorResponce? ErrorResponce { get; set; }
        public List<TranslatorResponceItem>? Items { get; set; }

    }
}
