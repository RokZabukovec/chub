using System;
using Newtonsoft.Json;

namespace chub.Models;

public class Command
{
    [JsonProperty(PropertyName = "command")]
    public string Name { get; set; }
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "pre")] public string Pre { get; set; } = string.Empty;
}