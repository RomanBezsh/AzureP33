using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    public class LangData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("nativeName")]
        public string NativeName { get; set; } = null!;

        [JsonPropertyName("code")]
        public string? Code { get; set; } = null!;

        [JsonPropertyName("dir")]
        public string? Dir { get; set; } = null!;

        [JsonPropertyName("scripts")]
        public List<LangData>? Scripts { get; set; } = null!;

        [JsonPropertyName("toScripts")]
        public List<LangData>? ToScripts { get; set; } = null!;
    }
}
