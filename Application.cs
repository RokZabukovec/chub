using chub.Cli;
using chub.Commands;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Cli;
using Spectre.Console;

namespace chub
{
    public class Application
    {
        public int Run(string[] args, IServiceCollection services)
        {
            if (args.Length == 0)
            {
                AnsiConsole.Write(new FigletText("CommandHub")
                    .LeftAligned()
                    .Color(Color.Blue));
            }
            
            var registrar = new TypeRegistrar(services);

            var app = new CommandApp(registrar);

            app.Configure(config =>
            {
                config.AddCommand<Login>("login")
                    .WithDescription("Login with CommandHub token.");
                config.AddCommand<Search>("search")
                    .WithDescription("Search for commands.");
            });

            return app.Run(args);
        }
    }
}
