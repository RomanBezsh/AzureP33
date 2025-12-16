using Microsoft.AspNetCore.Mvc;

namespace AzureP33.Models.Home
{
    public class HomeIndexFormModel
    {
        [FromQuery(Name = "lang-from")]
        public string LangFrom { get; set; } = null!;

        [FromQuery(Name = "lang-to")]
        public string LangTo { get; set; } = null!;

        [FromQuery(Name = "original-text")]
        public string OriginalText { get; set; } = null!;

        [FromQuery(Name = "action-button")]
        public string? Action { get; set; } = null!;
    }
}
