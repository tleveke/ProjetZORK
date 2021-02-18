using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using ProjetZORK.Services.Extensions;

using System.Threading.Tasks;
using ProjetZORK.DataAccessLayer.Extensions;
using ProjetZORK.theGame;
using Microsoft.Extensions.Logging;

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

            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) => {
                services.AddScoped<Launcher>().AddScoped<Game>().AddScoped<SetupGame>().AddDataService()/*/.AddLogging(
                        builder =>
                        {
                            builder.AddFilter("Microsoft", LogLevel.Warning)
                                   .AddFilter("System", LogLevel.Warning)
                                   .AddFilter("NToastNotify", LogLevel.Warning)
                                   .AddConsole();
                        })*/;
            });
        }
    }
}
