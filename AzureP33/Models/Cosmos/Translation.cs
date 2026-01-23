using Newtonsoft.Json;

namespace AzureP33.Models.Cosmos
{
    public class Translation
    {
        public static readonly string PartitionKey = "Translation";

        [JsonProperty("categoryId")]
        public string CategoryId = PartitionKey;

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("userID")]
        public int? UserId { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("from")]
        public Block From { get; set; } = null!;

        [JsonProperty("to")]
        public Block To { get; set; } = null!;
    }
}
