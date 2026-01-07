using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    public class LanguagesResponse
    {
        [JsonPropertyName("translation")]
        public Dictionary<string, LangData> Translations { get; set; } = new();
    }
}
