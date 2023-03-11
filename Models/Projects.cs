using System.Collections.Generic;
using Newtonsoft.Json;

namespace chub.Models;

public class Projects
{
    [JsonProperty(PropertyName = "projects")]
    public IEnumerable<Project> ProjectList { get; set; }
}