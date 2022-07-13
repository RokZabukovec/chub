using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Flock.Dtos;
using Flock.Exceptions;
using Flock.Responses;
using Flock.Services;
using Spectre.Cli;
using Spectre.Console;

namespace Flock.Commands;

public class Search  : AsyncCommand<Settings>
{
    private CommandService _commandService;
    private IAuthentication _auth;
    
    public Search(CommandService commandService, IAuthentication authentication)
    {
        _commandService = commandService;
        _auth = authentication;
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
           _auth.ReadUserCredentials();
        }
        catch (Exception e)
        {
            Console.WriteLine("You are not logged in yet. Run flock --help.");
            return 0;
        }
        
        Console.Clear();
        var results = new CommandResponse();
        var query = settings.Query;
        if (query == "*")
        {
            query = AnsiConsole.Ask<string>(Emoji.Replace($"{Emoji.Known.HourglassNotDone} [green]Search for: [/]"));
        }
        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .SpinnerStyle(Style.Parse("green bold"))
            .StartAsync(Emoji.Replace($"{Emoji.Known.LightBulb} [yellow]Thinking...[/]"), async ctx =>
            {
                results = await _commandService.Search(query);
                Thread.Sleep(800);
                return 0;
            });
        
        if (results != null && results.Data.Count > 0)
        {
            bool run = true;
            while (run)
            {
                var commands = results.Data;
                var selectedCommand = _commandService.ShowCommandSelectList(commands);
                var description = commands
                    .FirstOrDefault(command => command.Name == selectedCommand)
                    ?.Description;
                AnsiConsole.Markup($"[black on yellow]\n\n{description}\n\n[/]");
                var commandWithReplacedPlaceholders = _commandService.ReplaceParameters(selectedCommand);
                if (AnsiConsole.Confirm("Run command?"))
                {
                    Console.Clear();
                    _commandService.ExecuteCommand(commandWithReplacedPlaceholders);
                    run = false;
                    return 0;
                }
                Console.Clear();
            }
            return 0;
        }
        Console.WriteLine("No results returned.");
        return 0;
    }

}