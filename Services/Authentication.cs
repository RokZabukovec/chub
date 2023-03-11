using System;
using System.IO;
using Newtonsoft.Json;
using Spectre.Console;
using System.Text;
using chub.Dtos;
using chub.Exceptions;
using chub.Models;

namespace chub.Services
{
    public class Authentication : IAuthentication
    {
        private string AskForToken()
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]API token[/]?")
                .PromptStyle("green"));
        }

        public User AskForCredentials()
        {
            var user = new User();
            user.Token = AskForToken();
            return user;
        }

        public DirectoryInfo CreateCredentialsDirectory()
        {
            var dirName = ".chub";
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
