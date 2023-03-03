using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using chub.Services;

namespace chub.Requests
{
    internal class Request
    {

        public HttpClient Client { get; }
        public IConfiguration Config { get; }
        public IAuthentication Authentication { get; }

        public Request(HttpClient client, IConfiguration config, IAuthentication Authentication)
        {
            Client = client;
            Client.BaseAddress = new Uri("https://command-hub.si");
        }
    }
}
