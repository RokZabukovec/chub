﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using opti.Services;

namespace opti.Requests
{
    internal class Request
    {

        public HttpClient Client { get; }
        public IConfiguration Config { get; }
        public IAuthentication Authentication { get; }

        public Request(HttpClient client, IConfiguration config, IAuthentication Authentication)
        {
            Client = client;
            Client.BaseAddress = new Uri(Config.GetValue<string>("BaseUrl"));
        }
    }
}
