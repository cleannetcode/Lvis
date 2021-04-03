using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Modules;
using YouTubeChatBot.Managers;
using YouTubeChatBot.Services;

namespace YouTubeChatBot
{
    partial class Startup
    {
        public void ConfigureModules()
        {

        }
        public void AddServices(DICargo services)
        {
            services.RegisterSingleton<ConfigurationService>();
            services.RegisterSingleton<FileService>();
            services.RegisterSingleton<NetService>();
            services.RegisterSingleton<SerializeService>();
            services.RegisterSingleton(b => new TimeService(b.GetObject<ConfigurationService>()));
        }
    }
}
