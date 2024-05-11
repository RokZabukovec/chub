using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using Newtonsoft.Json;
using Spectre.Console;
using System.Text;
using System.Threading.Tasks;
using chub.Dtos;
using chub.Exceptions;
using chub.Models;
using Microsoft.Extensions.Configuration;

namespace chub.Services
{
    public class Authentication : IAuthentication
    {
        private readonly IConfiguration _configuration;

        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<UserDto> GetUser(string token)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.BaseAddress = new Uri(_configuration.GetValue<string>("BaseUrl"));
        
                var response = await client.GetAsync("/api/user");

                response.EnsureSuccessStatusCode();
        
                var responseBody = await response.Content.ReadAsStringAsync();
        
                var user = JsonConvert.DeserializeObject<UserDto>(responseBody);
                user.Token = token;

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        private string AskForToken()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]API token[/]?")
                .PromptStyle("green"));
        }

        public User AskForCredentials()
        {
            var user = new User
            {
                Token = AskForToken()
            };

            return user;
        }

        private DirectoryInfo CreateCredentialsDirectory()
        {
            const string dirName = ".chub";
            var userLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var location = Path.Combine(userLocation, dirName);
            if (!Directory.Exists(location))
            {
               return Directory.CreateDirectory(location);
            }
            return new DirectoryInfo(location);
        }

        public void CreateCredentialsFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                using (FileStream fs = File.Create(filePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("");
                    fs.Write(info, 0, info.Length);
                }

            }
        }

        public bool PersistCredentials(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            var dir = CreateCredentialsDirectory();
            var filePath = Path.Combine(dir.ToString(), "chub.json");
            using (FileStream fs = File.Create(filePath))
            {
                try
                {
                    fs.Close();
                    string json = JsonConvert.SerializeObject(user);
                    File.WriteAllText(filePath, json);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public UserDto ReadUserCredentials()
        {
            var dir = CreateCredentialsDirectory();
            var filePath = Path.Combine(dir.ToString(), "chub.json");

            if (File.Exists(filePath) == false)
            {
                throw new UnauthorizedException();
            }
            
            // deserialize JSON directly from a file
            using StreamReader file = File.OpenText(filePath);
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Deserialize(file, typeof(UserDto)) as UserDto;
        }
        
    }
}
