using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    //ORM for { "text":"Greetings","to":"en"}
    public class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = null!;

        [JsonPropertyName("to")]
        public string ToLang { get; set; } = null!;
    }
}
