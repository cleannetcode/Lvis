using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Services;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;
using Microsoft.Extensions.Logging;
using Google.Apis.YouTube.v3;


namespace LvisBot.YouTube.API
{
    public class YoutubeListener : ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>
    {
        private readonly INetService _service;
        private readonly ILogger _logger;

        public YoutubeListener(INetService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event Action<YTMessageResponse> OnMessageReceived;
        public event Action<StatusResponse> OnStatusReceived;
        public void Run(YouTubeConfig configuration)
        {
            RunAsync(configuration, CancellationToken.None).Wait();
        }

        public async Task RunAsync(YouTubeConfig configuration, CancellationToken token)
        {
            Console.WriteLine("RunAsync");

            var youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "",
                ApplicationName = "Cleannetcode"
            });
            
            var RequestVideoData = youTubeService.Videos.List("snippet,statistics,liveStreamingDetails");
                RequestVideoData.Id = "5yx6BWlEVcY";

            var ResponseVideoData = RequestVideoData.Execute();

            var videoTitle = ResponseVideoData.Items[0].Snippet.Title;
            var videoPublishedAt = ResponseVideoData.Items[0].Snippet.PublishedAt.Value.Date.ToString();
            var idLiveChat = ResponseVideoData.Items[0].LiveStreamingDetails.ActiveLiveChatId;

            var videoCountLikes = ResponseVideoData.Items[0].Statistics.LikeCount.ToString();
            var videoCountView = ResponseVideoData.Items[0].Statistics.ViewCount.ToString();

            var RequestLiveChatData = youTubeService.LiveChatMessages.List(idLiveChat,"snippet,AuthorDetails");
                
            var ResponseLiveChatData = RequestLiveChatData.Execute();

            var comments = new List<string>();
            foreach (var item in ResponseLiveChatData.Items)
            {
                comments.Add(item?.AuthorDetails.DisplayName + " : " + item.Snippet.DisplayMessage);
                Console.WriteLine(item?.AuthorDetails.DisplayName + " : " + item.Snippet.DisplayMessage);
            }

            await new Task(null);
        }
    }
}