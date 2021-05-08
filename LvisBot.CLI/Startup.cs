using LvisBot.BusinessLogic.Managers;
using LvisBot.BusinessLogic.Modules;
using LvisBot.BusinessLogic.Services;
using LvisBot.CargoDI;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;
using LvisBot.YouTube;
using Microsoft.Extensions.Logging;

namespace LvisBot.CLI
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(ILogger logger)
        {
            _logger = logger;
        }
        
        public void ConfigureServices(CargoCollection services)
        {
            services.RegisterSingleton(b => new ConfigurationService(b.GetObject<SerializationService>()));
            services.RegisterSingleton(b => new FileService(b.GetObject<ConfigurationService>()));
            services.RegisterSingleton<NetService>();
            services.RegisterSingleton<SerializationService>();
            services.RegisterSingleton(b => new TimeService(b.GetObject<ConfigurationService>()));

            services.RegisterSingleton<ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>, YoutubeListener>
                (b => new YoutubeListener(b.GetObject<NetService>(), _logger));

            services.RegisterSingleton
                (b => new QuestionHandler(b.GetObject<FileService>(), b.GetObject<SerializationService>(), b.GetObject<ConfigurationService>()));

            services.RegisterSingleton
                (b => new TimeCodeHandler(b.GetObject<FileService>(), b.GetObject<SerializationService>(), b.GetObject<ConfigurationService>()));

            services.RegisterSingleton
                (b => new CheckHandler(b.GetObject<FileService>(), b.GetObject<SerializationService>(), b.GetObject<ConfigurationService>()));

            services.RegisterSingleton(b => new YTModuleManager(b.GetObject<ConfigurationService>(), m =>
            {
                m.AddListener(b.GetObject<YoutubeListener>);
                m.AddModule(b.GetObject<QuestionHandler>);
                m.AddModule(b.GetObject<TimeCodeHandler>);
                m.AddModule(b.GetObject<CheckHandler>);
            }));
        }
    }
}
