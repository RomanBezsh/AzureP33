using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    public class TransliteratorResponseItem
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = null!;
        [JsonPropertyName("script")]
        public string Script { get; set; } = null!;
    }
}
