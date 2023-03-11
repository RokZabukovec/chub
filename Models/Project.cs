using Newtonsoft.Json;

namespace chub.Models;

public class Project
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    public override string ToString()
    {
        return Name;
    }
}