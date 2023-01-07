using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chub.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
