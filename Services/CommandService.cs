using System.Diagnostics;
using System.Text.RegularExpressions;
using chub.Models;
using chub.Responses;
using chub.Requests;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Spectre.Console;

namespace chub.Services;

public class CommandService : ISearchService
{
    private IConfiguration _config;
    private IAuthentication _auth;

    public CommandService(IConfiguration configuration, IAuthentication authentication)
    {
        _config = configuration;
        _auth = authentication;
    }
    
    public async Task<CommandResponse> Search(string query)
    {
        var user = _auth.ReadUserCredentials();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {user.Token}");
        client.BaseAddress = new Uri(_config.GetValue<string>("BaseUrl"));
        
        var response = await client.GetAsync($"api/search?q={query}");
        
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CommandResponse>(responseBody);
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
    
    public void ExecuteCommand(string command)
    {
        Process proc = new Process ();
        proc.StartInfo.FileName = "/bin/bash";
        proc.StartInfo.Arguments = "-c \" " + command + " \"";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start ();

        while (!proc.StandardOutput.EndOfStream) {
            Console.WriteLine (proc.StandardOutput.ReadLine ());
        }
    }

    public string ReplaceParameters(string command)
    {
        // Matches a string that is between <> and any number of characters.
        // It's used as a placeholder format for parameters.
        // example: <query> => result: query
        Regex regex = new Regex("(?<=<).*?(?=>)");
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