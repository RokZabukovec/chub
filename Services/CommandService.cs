using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using chub.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Spectre.Console;

namespace chub.Services;

public class CommandService(IConfiguration configuration, IAuthentication authentication)
    : ISearchService
{
    public async Task<IEnumerable<Command>> Search(string query)
    {
        var user = authentication.ReadUserCredentials();
        var client = new HttpClient();
        
        client.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {user.Token}");
        client.BaseAddress = new Uri(configuration.GetValue<string>("BaseUrl"));
        
        var response = await client.GetAsync($"/api/chub?q={query}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<IEnumerable<Command>>(responseBody);
    }

    public string ShowCommandSelectList(IEnumerable<Command> commands)
    {
        var commandNames = commands.Select(command => command.Name).ToList();
        Console.Title = $"chub found {commandNames.Count} results.";
        var command = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]These are the results:[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more commands.)[/]")
                .AddChoices(commandNames));
        
        return command;
    }

    public List<string> ExecuteCommand(string command)
    {
        var outputLines = new List<string>();

        if (string.IsNullOrWhiteSpace(command) == false)
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                }
            };
            
            if (OperatingSystem.IsWindows())
            {
                proc.StartInfo.FileName = "cmd.exe"; // Use cmd.exe on Windows
                proc.StartInfo.Arguments = $"/C \"{command}\""; // Pass the command as an argument to cmd.exe
            }
            else
            {
                proc.StartInfo.FileName = "/bin/bash"; // Use bash on Unix-like systems
                proc.StartInfo.Arguments = $"-c \"{command}\""; // Pass the command as an argument to bash
            }
            
            proc.Start();
            
            while (!proc.HasExited)
            {
                var line = proc.StandardOutput.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                Console.WriteLine(line);
            }

            //proc.WaitForExit();

            return outputLines;
        }
        
        return outputLines;
    }
    
    public static string ReplaceParameters(string command)
    {
        // Matches a string that is between <> and any number of characters.
        // It's used as a placeholder format for parameters.
        // example: <query> => result: query
        var regex = new Regex("(?<=<).*?(?=>)");
        var matches = regex.Matches(command).Distinct().ToList();
        if (matches.Count > 0)
        {
            AnsiConsole.WriteLine("Parameters: ");
            foreach (Match match in regex.Matches(command))
            {
                var placeholderFormatForReplacing = $"<{match.Value}>";
                var parameterValue = AnsiConsole.Ask<string>($"[green]{match.Value}[/]: ");
                command = command.Replace(placeholderFormatForReplacing, parameterValue);
            }
        }

        return command;
    }
}