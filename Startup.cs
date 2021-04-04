using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Modules;
using YouTubeChatBot.Managers;
using YouTubeChatBot.Services;
using YouTubeChatBot.YouTube;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Models;

namespace YouTubeChatBot
{
    partial class Startup
    {
        private void RunWith(DICargo services)
        {
            services.GetObject<YTModuleManager>().Run();
        }
        private void ConfigureServices(DICargo services)
        {
            services.RegisterSingleton(b => new ConfigurationService(b.GetObject<SerializeService>()));
            services.RegisterSingleton(b => new FileService(b.GetObject<ConfigurationService>()));
            services.RegisterSingleton<NetService>();
            services.RegisterSingleton<SerializeService>();
            services.RegisterSingleton(b => new TimeService(b.GetObject<ConfigurationService>()));

            services.RegisterSingleton<ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>, YoutubeListener>
                (b => new YoutubeListener(b.GetObject<NetService>()));

            services.RegisterSingleton
                (b => new QuestionHandler(b.GetObject<FileService>(), b.GetObject<SerializeService>(), b.GetObject<ConfigurationService>()));

            services.RegisterSingleton
                (b => new TimeCodeHandler(b.GetObject<FileService>(), b.GetObject<SerializeService>(), b.GetObject<ConfigurationService>()));

            services.RegisterSingleton(b => new YTModuleManager(b.GetObject<ConfigurationService>(), m =>
            {
                m.AddListener(b.GetObject<YoutubeListener>);
                m.AddModule(b.GetObject<QuestionHandler>);
                m.AddModule(b.GetObject<TimeCodeHandler>);
            }));
        }
    }
}
