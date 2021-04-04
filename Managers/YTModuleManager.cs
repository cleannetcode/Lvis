using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Models;
using YouTubeChatBot.Services;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeChatBot.Managers
{
    class YTModuleManager : ModuleManager<string, YTMessageResponse, StatusResponse, YouTubeConfig>
    {
        char keyPrefix;
        YouTubeConfig ytConfig;
        CancellationTokenSource token;
        public YTModuleManager(ConfigurationService configurationService, 
            Action<ModuleManager<string, YTMessageResponse, StatusResponse, YouTubeConfig>> configure) : base(configure)
        {
            keyPrefix = configurationService.ChatKeySymbol;
            ytConfig = configurationService.YouTubeConfig;
        }
        public YTModuleManager(ConfigurationService configurationService) : this(configurationService, null) { }
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
            return mess.Context.Split(" ")[0][1..];
        }
        public void Run()
        {
            Run(ytConfig);
            token = new CancellationTokenSource();
            var endTask = new Task(() => { while (true) Thread.Sleep(1000000000); }, token.Token);
            endTask.Wait();
        }
        protected override void StateHandle(ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse> sender, StatusResponse StatusModel)
        {
            Console.WriteLine(StatusModel.State);
            if (StatusModel.Code >= 400)
            {
                token.Cancel();
            }
        }
    }
}
