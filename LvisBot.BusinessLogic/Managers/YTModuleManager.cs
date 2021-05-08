using System;
using System.Threading.Tasks;
using LvisBot.BusinessLogic.Services;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Managers
{
    // ReSharper disable once InconsistentNaming
    public class YTModuleManager : ModuleManager<string, YTMessageResponse, StatusResponse, YouTubeConfig>
    {
        private readonly char _keyPrefix;
        private readonly YouTubeConfig _ytConfig;
        
        public YTModuleManager(ConfigurationService configurationService, 
            Action<ModuleManager<string, YTMessageResponse, StatusResponse, YouTubeConfig>> configure) : base(configure)
        {
            _keyPrefix = configurationService.ChatKeySymbol;
            _ytConfig = configurationService.YouTubeConfig;
        }
        public YTModuleManager(ConfigurationService configurationService) : this(configurationService, null) { }
        
        public void Run()
        {
            Run(_ytConfig);
        }
        
        public async Task RunAsync()
        {
            await Task.Run(Run);
        }
        
        protected override string GetPrefix(YTMessageResponse mess)
        {
            if (mess?.Context == null || mess.Context == string.Empty)
            {
                return null;
            }
            if (mess.Context[0] != _keyPrefix)
            {
                return null;
            }
            return mess.Context.Split(" ")[0][1..];
        }
        
        protected override void OnStatusReceived(ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse> sender, StatusResponse statusModel)
        {
            Console.WriteLine($"Code: {statusModel.Code} Status: {statusModel.State}");
        }
    }
}
