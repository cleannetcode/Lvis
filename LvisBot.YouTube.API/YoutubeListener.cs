using System;
using System.Threading;
using System.Threading.Tasks;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.YouTube.API
{
    public class YoutubeListener : ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event Action<YTMessageResponse> OnMessageReceived;
        public event Action<StatusResponse> OnStatusReceived;
        public void Run(YouTubeConfig configuration)
        {
            throw new NotImplementedException();
        }

        public Task RunAsync(YouTubeConfig configuration, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}