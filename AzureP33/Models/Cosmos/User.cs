using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AzureP33.Models.Cosmos
{
    public class User
    {
        public static readonly string PartitionKey = "User";

        [JsonProperty("categoryId")]
        public string CategoryId = User.PartitionKey;

        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
