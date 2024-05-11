using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using chub.Services;


namespace chub.Requests;

internal class LoginRequest
{
    public IAuthentication _auth { get; set; }

    public IConfiguration _config;

    public LoginRequest(IAuthentication Authentication, IConfiguration Configuration)
    {
        _auth = Authentication;
        _config = Configuration;
    }

    public async Task<bool> Login()
    {
        var userToken = _auth.AskForCredentials();

        var actualUser = await _auth.GetUser(userToken.Token);

        if (actualUser is null)
        {
            Console.WriteLine($"The token ({userToken.Token}) you provided is not valid.");
                
            return false;
        }
            
        var success = _auth.PersistCredentials(userToken);
        if (!success) return false;
            
        Console.WriteLine("You are now logged in.");
            
        return true;

    }
}