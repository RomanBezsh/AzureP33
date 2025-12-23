using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    public class ErrorData
    {
        [JsonPropertyName("code")]
        public int? Code { set; get; } = null;


        [JsonPropertyName("message")]
        public string Message { set; get; } = null!;
    }
}
