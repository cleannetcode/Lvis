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

        private VideoData GetListDataVideo(string idVideo)
        {
            var RequestVideoData = _youTubeService.Videos.List("snippet,statistics,liveStreamingDetails");
                RequestVideoData.Id = idVideo;
            
            var ResponseVideoData = RequestVideoData.Execute();
            VideoData result = null;
            
            if (ResponseVideoData.Items.Count > 0)
            {
                var item = ResponseVideoData.Items[0];
                result = new VideoData(
                    item.Snippet.Title,
                    item.Snippet.PublishedAt.Value.Date.ToString(),
                    item.LiveStreamingDetails.ActiveLiveChatId,
                    item.Statistics.LikeCount.ToString(),
                    item.Statistics.ViewCount.ToString());
            }
            return result;
        }

        private void GetListCommentsVideo(string idLiveChat)
        {
            var RequestLiveChatData = _youTubeService.LiveChatMessages.List(idLiveChat,"snippet,AuthorDetails");
                
            var ResponseLiveChatData = RequestLiveChatData.Execute();
            
            var comments = new List<string>();
            foreach (var item in ResponseLiveChatData.Items)
            {
                comments.Add(item?.AuthorDetails.DisplayName + " : " + item.Snippet.DisplayMessage);
                Console.WriteLine(item?.AuthorDetails.DisplayName + " : " + item.Snippet.DisplayMessage);
            }
        }
        public async Task RunAsync(YouTubeConfig configuration, CancellationToken token)
        {
            Console.WriteLine("RunAsync");
            
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
            
            
            //GetListCommentsVideo();
       
            await new Task(null);
        }
        internal class VideoData
        {
            public string VideoTitle { get; }
            public string VideoPublishedAt { get; }
            public string IdLiveChat { get; }
            public string VideoCountLikes { get; }
            public string VideoCountView { get; } 
            public VideoData(
                string videoTitle, 
                string videoPublishedAt,
                string idLiveChat,
                string videoCountLikes,
                string videoCountView)
            {
                VideoTitle = videoTitle;
                VideoPublishedAt = videoPublishedAt;
                IdLiveChat = idLiveChat;
                VideoCountLikes = videoCountLikes;
                VideoCountView = videoCountView;
            }
           
        }
    }
}