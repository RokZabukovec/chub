using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using chub.Responses;
using chub.Services;
using Spectre.Cli;
using Spectre.Console;

namespace chub.Commands;

public class Save  : AsyncCommand<Settings>
{
    private CommandService _commandService;
    private IAuthentication _auth;
    
    public Save(CommandService commandService, IAuthentication authentication)
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
        
        return 0;
    }

}