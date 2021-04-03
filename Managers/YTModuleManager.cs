using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Models;

namespace YouTubeChatBot.Managers
{
    class YTModuleManager : ModuleManager<string, YTMessageResponse, StatusResponse, YouTubeConfig>
    {
        public YTModuleManager(params ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>[] listeners) : base(GetPrefix, listeners)
        {

        }
        private static string GetPrefix(YTMessageResponse mess)
        {
            return mess.Context.Split(" ")[0];
        }
        protected override void StateHandle(ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse> sender, StatusResponse StatusModel)
        {
            throw new NotImplementedException();
        }
    }
}
