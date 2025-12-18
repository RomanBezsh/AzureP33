using AzureP33.Models.Orm;

namespace AzureP33.Models.Home
{
    public class HomeIndexViewModel
    {
        public string PageTitle { get; set; } = "";
        public HomeIndexFormModel? FormModel { get; set; }
        public LanguagesResponce LanguagesResponce { get; set; } = null!;

    }
}
