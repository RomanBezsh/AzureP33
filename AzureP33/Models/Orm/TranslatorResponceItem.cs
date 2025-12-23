using System.Text.Json.Serialization;

namespace AzureP33.Models.Orm
{
    //ORM for { "translations":[{ "text":"Greetings","to":"en"}]}
    public class TranslatorResponceItem
    {
        [JsonPropertyName("tranlations")]
        public List<Translation> Translations { set; get; } = new();

    }
}
