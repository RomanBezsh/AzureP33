using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    public class TranslatorErrorResponce
    {
        [JsonPropertyName("error")]
        public ErrorData Error { get; set; } = null!;
    }
}
