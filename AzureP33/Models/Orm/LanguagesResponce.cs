using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    public class LanguagesResponce
    {
        [JsonPropertyName("translation")]
        public Dictionary<string, LangData> Translation { get; set; } = new();
    }
}
