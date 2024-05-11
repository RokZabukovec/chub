using System;
using System.Linq;
using System.Threading.Tasks;
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
        var results = Enumerable.Empty<Command>();
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
                results = (await _commandService.Search(query)).ToList();
                
                return 0;
            });

        var commands = results.ToList();

        if (commands.Count == 0 )
        {
            var currentCommand = new Command
            {
                Name = query,
                Description = "Currently entered search query can be run if there are no results",
                Pre = string.Empty
            };
            
            commands.Add(currentCommand);
        }
        
        while (true)
        {
            var selectedCommand = _commandService.ShowCommandSelectList(commands);
            var description = commands
                .FirstOrDefault(command => command.Name == selectedCommand)
                ?.Description;
                
            var pre = commands
                .FirstOrDefault(command => command.Name == selectedCommand)
                ?.Pre;
            if (string.IsNullOrEmpty(description) == false)
            {
                AnsiConsole.Markup($"[black on yellow]\n\n{description ?? ""}\n\n[/]");
            }
                
            if (string.IsNullOrEmpty(pre) == false)
            {
                AnsiConsole.Write($"{pre} \n");
            }
                
            var commandWithReplacedPlaceholders = CommandService.ReplaceParameters(selectedCommand);
            if (AnsiConsole.Confirm("Run command?"))
            {
                Console.Clear();
                _commandService.ExecuteCommand(commandWithReplacedPlaceholders);
                
                break;
            }
                
            Console.Clear();
        }
        
        return 0;
    }

}