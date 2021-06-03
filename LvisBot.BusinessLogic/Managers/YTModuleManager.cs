using System;
using System.Threading;
using System.Threading.Tasks;
using LvisBot.BusinessLogic.Services;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Managers
{
    // ReSharper disable once InconsistentNaming
    public class YTModuleManager : ModuleManager<string, StatusResponse, YouTubeConfig>
    {
        private readonly char _keyPrefix;
        private readonly YouTubeConfig _ytConfig;
        private IMemberService _memberService;
        
        public YTModuleManager(ConfigurationService configurationService, 
            Action<ModuleManager<string, StatusResponse, YouTubeConfig>> configure,
            IMemberService memberService) : base(configure, memberService)
        {
            _keyPrefix = configurationService.ChatKeySymbol;
            _ytConfig = configurationService.YouTubeConfig;
            _memberService = memberService;
        }
        public YTModuleManager(ConfigurationService configurationService) : this(configurationService, null, null) { }
        
        public void Run()
        {
            Run(_ytConfig);
        }
        
        public async Task RunAsync(CancellationToken token)
        {
            await Task.Run(Run, token);
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
