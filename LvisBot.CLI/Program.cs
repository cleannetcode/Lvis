using LvisBot.BusinessLogic.Managers;
using LvisBot.CargoDI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LvisBot.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(x => new Startup(x.GetService<ILogger>()));
                    
                    services.AddHostedService(x =>
                    {
                        var cargoCollection = new CargoCollection();
                        var startup = x.GetService<Startup>();
                        startup.ConfigureServices(cargoCollection);
                        var manager = cargoCollection.GetObject<YTModuleManager>();

                        return new Worker(x.GetService<ILogger<Worker>>(), manager);
                    });
                });
    }
}