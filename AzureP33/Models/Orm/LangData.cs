using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    public class LangData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; } = null!;
        [JsonPropertyName("dir")]
        public string? Dir { get; set; } = null!;
    }
}
