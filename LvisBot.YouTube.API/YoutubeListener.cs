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
        private YouTubeService _youTubeService;

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

        private List<string> GetListLiveVideo(string channelId)
        {
            var searchListRequest = _youTubeService.Search.List("snippet");
            searchListRequest.ChannelId = channelId; 
            // "UCOxqgCwgOqC2lMqC5PYz_Dg";// Chillhop Music
            // "UCbOgH6XlQURq9PnGfa4L4Mw";//Roman Trufanov
            searchListRequest.MaxResults = 2;
            searchListRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            searchListRequest.Type = "video";
                
            
            //var searchListResponse = await searchListRequest.ExecuteAsync();
            var searchListResponse = searchListRequest.Execute();
            
            var responseData = new List<string>();
            foreach (var item in searchListResponse.Items)
            {
                responseData.Add(item.Id.VideoId);
                //item.Snippet.PublishedAt.Value.ToString();
                // item.Snippet.Title;
                // item.Snippet.Description;
                //Console.WriteLine(item.Id + " : " + item.Snippet.Title);
            }

            return responseData;
        }

        private YTVideoDataResponse GetListDataVideo(string idVideo)
        {
            var RequestVideoData = _youTubeService.Videos.List("snippet,statistics,liveStreamingDetails");
                RequestVideoData.Id = idVideo;
            
            var ResponseVideoData = RequestVideoData.Execute();
            YTVideoDataResponse result = null;
            
            if (ResponseVideoData.Items.Count > 0)
            {
                var item = ResponseVideoData.Items[0];
                result = new YTVideoDataResponse(
                    item.Snippet.Title, 
                    item.Snippet.PublishedAt.Value,
                    item.LiveStreamingDetails.ActiveLiveChatId,
                    item.Statistics.LikeCount,
                    item.Statistics.ViewCount);
            }
            return result;
        }

        private List<YTMessageResponse> GetListCommentsVideo(string idLiveChat, DateTime startStream)
        {
            var RequestLiveChatData = _youTubeService.LiveChatMessages.List(idLiveChat,"snippet,AuthorDetails");
                
            var ResponseLiveChatData = RequestLiveChatData.Execute();

            //pagination via request by chat
            //var nextPageToken = ResponseLiveChatData.NextPageToken;
            // var totalResult = ResponseLiveChatData.PageInfo.ResultsPerPage;
            // var resultPerPage = ResponseLiveChatData.PageInfo.ResultsPerPage; 

            List<YTMessageResponse> result = new List<YTMessageResponse>();
            foreach (var item in ResponseLiveChatData.Items)
            {
                var userType = 
                    item.AuthorDetails.IsChatOwner.Value ? YTMessageResponse.AuthorTypes.Owner :
                    item.AuthorDetails.IsChatModerator.Value ? YTMessageResponse.AuthorTypes.Moderator :
                    item.AuthorDetails.IsVerified.Value ? YTMessageResponse.AuthorTypes.Verified :
                    item.AuthorDetails.IsChatSponsor.Value ? YTMessageResponse.AuthorTypes.Sponsor :
                    YTMessageResponse.AuthorTypes.Other;

                result.Add(new YTMessageResponse(
                    item.Id,
                    item.AuthorDetails.DisplayName,
                    item.AuthorDetails.ChannelId,
                    item.Snippet.DisplayMessage,
                    item.Snippet.PublishedAt.Value,
                    startStream,
                    userType));
                
                //item.AuthorDetails.ProfileImageUrl
            }

            return result;
        }
        public async Task RunAsync(YouTubeConfig configuration, CancellationToken token)
        {
            try
            {
                _youTubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = configuration.ApiKey,
                    ApplicationName = configuration.ApplicationName
                });
            
                var liveStreams = GetListLiveVideo(configuration.ChannelID);
                if (liveStreams.Count == 0)
                {
                    OnStatusReceived?.Invoke(new StatusResponse(404, "Stream not founded"));
                    return;
                }
            
                var videoData = GetListDataVideo(liveStreams[0]);
                if (videoData == null)
                {
                    OnStatusReceived?.Invoke(new StatusResponse(404, "Video info not founded"));
                    return;
                }
                
                OnStatusReceived?.Invoke(new StatusResponse(200, $"LiveChat started: {videoData.VideoTitle + " - " + videoData.VideoPublishedAt}"));
                var errors = 0;
                var utcInit = DateTime.UtcNow;
                DateTime? lastMessageTime = null;
                
                while (errors < 10)
                {
                    var messageResponses = new List<YTMessageResponse>();
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(TimeSpan.FromMilliseconds(configuration.UpdateMs));
                    token.ThrowIfCancellationRequested();
                    
                    DateTime? firstMessageTime = null;                    
                    var chatMessages = GetListCommentsVideo(videoData.IdLiveChat,videoData.VideoPublishedAt);
                    
                    if (chatMessages.Count == 0)
                    {
                        errors++;
                        continue;
                    }
                    chatMessages.Reverse();
                    foreach (var message in chatMessages)
                    {
                        if (lastMessageTime == null && message.UtcTime < utcInit) continue;
                        if (firstMessageTime == null) firstMessageTime = message.UtcTime;
                        if (lastMessageTime >= message.UtcTime) break;
                        messageResponses.Add(message);
                    }
                    
                    messageResponses.Reverse();
                    foreach (var messageResponse in messageResponses)
                    {
                        token.ThrowIfCancellationRequested();
                        Console.WriteLine($"{messageResponse.UserName}: {messageResponse.Context}");
                        var test = messageResponse.UserName;
                        OnMessageReceived?.Invoke(messageResponse);
                    }
                    
                    errors = 0;
                    lastMessageTime = firstMessageTime ?? lastMessageTime;
                    token.ThrowIfCancellationRequested();
                }

                if (errors > 10)
                {
                    OnStatusReceived?.Invoke(new StatusResponse(400, "LiveChat closed"));
                }
            }
            catch (Exception e)
            {
                OnStatusReceived?.Invoke(new StatusResponse(401, e.ToString()));
            }
        }
    }
}