using Newtonsoft.Json;

namespace AzureP33.Models.Cosmos
{
    public class Block
    {
        [JsonProperty("text")]
        public string Text { get; set; } = null!;

        [JsonProperty("language")]
        public string Language { get; set; } = null!;

        [JsonProperty("transliteration")]
        public TransliterationInfo? Transliteration { get; set; }
    }
}
