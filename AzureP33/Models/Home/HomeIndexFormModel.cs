using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AzureP33.Models.Home
{
    public class HomeIndexFormModel
    {
        [FromQuery(Name = "lang-from")]
        public string LangFrom { get; set; } = null!;

        [FromQuery(Name = "lang-to")]
        public string LangTo { get; set; } = null!;

        [Required(ErrorMessage = "Текст для перекладу обов'язковий")]
        [MinLength(2, ErrorMessage = "Текст має містити щонайменше 2 символи")]
        [FromQuery(Name = "original-text")]
        public string OriginalText { get; set; } = null!;

        [FromQuery(Name = "action-button")]
        public string? Action { get; set; } = null!;
    }
}
