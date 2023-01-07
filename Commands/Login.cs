using chub.Requests;
using chub.Services;
using Spectre.Cli;

namespace chub.Commands
{
    internal class Login : AsyncCommand<Settings>
    {
        protected IAuthentication _auth { get; set; }

        public LoginRequest _login { get; set; }

        public Login(IAuthentication auth, LoginRequest login)
        {
            _auth = auth;
            _login = login;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var login = await _login.Login();
            var query = settings.Query;
            if (string.IsNullOrEmpty(query))
            {
                Console.WriteLine("[red]Search query is empty.[/]");
                return 0;
            }
            return 0;
        }
    }
}
