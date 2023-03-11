using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using chub.Models;
using chub.Responses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Spectre.Console;

namespace chub.Services;

public class ProjectService : IProjectService
{
    private readonly IAuthentication _authentication;
    private readonly IConfiguration _configuration;

    public ProjectService(IAuthentication authentication, IConfiguration configuration)
    {
        _authentication = authentication;
        _configuration = configuration;
    }
    
    public async Task<Project> ShowProjects()
    {
        var projects = await GetProjects();
        var projectList = projects.Data.ProjectList.ToList();
        var project = AnsiConsole.Prompt(
            new SelectionPrompt<Project>()
                .Title("Select a project")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to select a project)[/]")
                .AddChoices(projectList));
        return project;
    }

    private async Task<ObjectResponse<Projects>> GetProjects()
    {
        var user = _authentication.ReadUserCredentials();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {user.Token}");
        client.BaseAddress = new Uri(_configuration.GetValue<string>("BaseUrl"));

        var response = await client.GetAsync("api/projects");

        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ObjectResponse<Projects>>(responseBody);
    }
}