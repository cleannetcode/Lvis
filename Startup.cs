using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Modules;
using YouTubeChatBot.Managers;

namespace YouTubeChatBot
{
    partial class Startup
    {
        public void ConfigureModules()
        {

        }
        public void AddServices(DICargo services)
        {
            services.RegisterSingleton<ModuleManager<Y>>()
        }
    }
}
