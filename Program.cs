﻿using System;
using System.IO;
using System.Threading.Tasks;
using chub.Requests;
using chub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


namespace chub
{
    public class Program
    {
        /// <summary>
        ///     Configuration for the application
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Entry point of the project.
        ///     Notice the method is returning async task instead of void in order to support async programming.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                Configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                                        .AddEnvironmentVariables()
                                        .Build();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom
                    .Configuration(Configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();

                // Create service collection and configure our services
                var services = ConfigureServices();
                var serviceProvider = services.BuildServiceProvider();
                var response = serviceProvider.GetService<Application>()?.Run(args, services);

                Log.Information("Shutting down...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        ///     Configure the services using ASP.NET CORE built-in Dependency Injection.
        /// </summary>
        /// <returns></returns>
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddSingleton<IAuthentication, Authentication>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddSingleton<CommandService>();
            services.AddTransient<LoginRequest>();
            services.AddOptions();
            services.AddTransient<Application>();
            return services;
        }
    }

}