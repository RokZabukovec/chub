using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using chub.Services;
using Spectre.Cli;
using Spectre.Console;

namespace chub.Commands;

public class Save : AsyncCommand<Settings>
{
    private CommandService _commandService;
    private IAuthentication _auth;
    private readonly IProjectService _projectService;

    public Save(CommandService commandService, IAuthentication authentication, IProjectService projectService)
    {
        _commandService = commandService;
        _auth = authentication;
        _projectService = projectService;
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

        if(OperatingSystem.IsWindows())
        {
            Console.WriteLine("Saving commands on Windows is not yet supported");
            return 0;
        }

        List<string> output = _commandService.ExecuteCommand("history");

        
        foreach (var s in output)
        {
            Console.WriteLine(s);
        }
        
        Console.Clear();
        Console.Title = "Save command";
        var loadingText = "Fetching projects";
        
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            loadingText = $"{Emoji.Known.LightBulb} [yellow]Fetching projects...[/]";
        }
        
        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .SpinnerStyle(Style.Parse("green bold"))
            .Start(Emoji.Replace(loadingText), async ctx =>
            {
                var project = await _projectService.ShowProjects();
                Console.WriteLine($"You selected {project}");
                return 0;
            });
        return 0;
    }

}