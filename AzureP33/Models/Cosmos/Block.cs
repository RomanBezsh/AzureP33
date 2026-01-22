using Newtonsoft.Json;

namespace AzureP33.Models.Cosmos
{
    public class Block
    {
        [JsonProperty("lang")]
        public string Lang { get; set; } = null!;
        [JsonProperty("text")]
        public string Text { get; set; } = null!;
        [JsonProperty("translitaration")]
        public Transliteration? Translitaration { get; set; } = null;
    }
}
