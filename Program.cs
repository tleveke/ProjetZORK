﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using ProjetZORK.Services.Extensions;
using ProjetZORK.DataAccessLayer.Extensions;

using System.Threading.Tasks;

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
            // new Menu();
            // Game game = new Game();
            // _ = game;
        }
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) => {
                services.AddSingleton<Launcher>();
                services.AddDataService();
             });
        }
    }
}
