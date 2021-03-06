﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using ProjetZORK.Services.Extensions;

using System.Threading.Tasks;
using ProjetZORK.DataAccessLayer.Extensions;
using Microsoft.Extensions.Logging;
using ProjetZORK.theGame;

namespace ProjetZORK
{
    class Program
    {
        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build().SeedDatabase();
            var run = host.RunAsync();
            var launcher = host.Services.GetService<Launcher>();
            launcher.Exit += (o, e) => host.StopAsync();
            launcher.Start();
            return run;
        }
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
            {
                services.AddSingleton<Launcher>().AddSingleton<Game>().AddDataService();
                    services.AddLogging(
                        builder =>
                        {
                            builder.AddFilter("Microsoft", LogLevel.Warning)
                                   .AddFilter("System", LogLevel.Warning)
                                   .AddFilter("NToastNotify", LogLevel.Warning)
                                   .AddConsole();
                        });
            });
        }
    }
}