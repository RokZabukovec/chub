using System;
using System.Linq;
using System.Threading.Tasks;
using chub.Responses;
using chub.Services;
using Spectre.Cli;
using Spectre.Console;
using Command = chub.Models.Command;

namespace chub.Commands;

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
        catch (Exception)
        {
            Console.WriteLine("You are not logged in yet. Run chub --help.");
            return 0;
        }
        
        Console.Clear();
        var results = new ListResponse<Command>();
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
                if (string.IsNullOrEmpty(description) == false)
                {
                    AnsiConsole.Markup(string.Format("[black on yellow]\n\n{0}\n\n[/]", description ?? ""));
                }
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
        }
        Console.WriteLine("No results returned.");
        return 0;
    }

}