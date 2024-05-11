using System.Text.Json.Serialization;

namespace chub.Models;

using System.Collections.Generic;

public class SearchResult
{
    [JsonPropertyName("data")]
    public Data Data { get; set; }
}

public class Data
{
    [JsonPropertyName("hits")]
    public List<Command> Hits { get; set; }
}
