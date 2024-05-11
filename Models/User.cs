using Newtonsoft.Json;

namespace chub.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
