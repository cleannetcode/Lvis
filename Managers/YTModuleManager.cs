using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Models;
using YouTubeChatBot.Services;

namespace YouTubeChatBot.Managers
{
    class YTModuleManager : ModuleManager<string, YTMessageResponse, StatusResponse, YouTubeConfig>
    {
        char keyPrefix;
        public YTModuleManager(ConfigurationService configurationService, 
            params ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>[] listeners) : base(listeners)
        {
            keyPrefix = configurationService.ChatKeySymbol;
        }
        protected override string GetPrefix(YTMessageResponse mess)
        {
            if (mess == null || mess.Context == null || mess.Context == string.Empty)
            {
                return null;
            }
            if (mess.Context[0] != keyPrefix)
            {
                return null;
            }
            return mess.Context.Split(" ")[0];
        }
        protected override void StateHandle(ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse> sender, StatusResponse StatusModel)
        {
            throw new NotImplementedException();
        }
    }
}
