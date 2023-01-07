using Newtonsoft.Json;

namespace chub.Models;

public class Command
{
    [JsonProperty(PropertyName = "command")]
    public string Name { get; set; }
    public string Description { get; set; }
}