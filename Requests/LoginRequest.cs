using chub.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using chub.Services;
using chub.Responses;
using chub.Dtos;

namespace chub.Requests
{
    internal class LoginRequest
    {
        public IAuthentication _auth { get; set; }

        public IConfiguration _config;

        public LoginRequest(IAuthentication Authentication, IConfiguration Configuration)
        {
            _auth = Authentication;
            _config = Configuration;
        }

        public async Task<bool> Login()
        {
            var user = _auth.AskForCredentials();

            var success = _auth.PersistCredentials(user);
            if (!success) return false;
            Console.WriteLine("You are now logged in.");
            return true;

        }
    }
}
