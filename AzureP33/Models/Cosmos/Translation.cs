using Newtonsoft.Json;

namespace AzureP33.Models.Cosmos
{
    public class Translation
    {
        public static readonly string PartitionKey = "Translation";

        [JsonProperty("categoryId")]
        public string CategoryId = Translation.PartitionKey;

        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("userID")]
        public int? UserId { get; set; }

        [JsonProperty("from")]
        public Block From { get; set; } = null!;
        [JsonProperty("to")]
        public Block To { get; set; } = null!;
    }
}
