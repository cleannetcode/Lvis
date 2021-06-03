using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LvisBot.Domain.Enums;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LvisBot.YouTube
{
    public class YoutubeListener : ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>
    {
        private static class YoutubeURL
        {
            public static Uri GetVideoInfo(string videoId) => new Uri($"{'h'}ttps://www.youtube.com/get_video_info?video_id={videoId}");
            public static Uri GetLiveChat(string videoId) => new Uri($"{'h'}ttps://www.youtube.com/live_chat?v={videoId}");
            public static Uri GetChannel(string channelId) => new Uri($"{'h'}ttps://www.youtube.com/channel/{channelId}");
        }

        private readonly INetService _service;
        private readonly ILogger _logger;
        private CancellationTokenSource _canceller;

        public YoutubeListener(INetService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public event Action<YTMessageResponse> OnMessageReceived;
        public event Action<StatusResponse> OnStatusReceived;

       
        public void Run(YouTubeConfig configuration)
        {
            RunAsync(configuration, CancellationToken.None).Wait();
        }
        public async Task RunAsync(YouTubeConfig configuration, CancellationToken token)
        {
            await RunTask(TimeSpan.FromMilliseconds(configuration.UpdateMs), configuration.ChannelID, token);
        }

        private async Task<NetResponse> GetYoutubeResponse(Uri url) => await _service.Request(url, RequestMethod.GET, new Dictionary<string, string>()
        {
            ["useragent"] = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
        });
        private async Task<JObject> GetYoutubeData(Uri url)
        {
            const string begin = "window[\"ytInitialData\"] = ";
            const string end = ";</script>";
            const string begin2 = "var ytInitialData = ";

            var response = await GetYoutubeResponse(url);
            
            if (response.StatusCode != 200) return null;
            
            var html = Encoding.Default.GetString(response.Data);
            var indxStart = html.IndexOf(begin);

            if (indxStart == -1) indxStart = html.IndexOf(begin2) + begin2.Length;
            else indxStart += begin.Length;

            var indxEnd = html.IndexOf(end, indxStart);
            var text = html[indxStart..indxEnd];
            var result = JObject.Parse(text);
            return result;
        }
        private async Task<Uri> GetFullLiveChat(Uri url)
        {
            var json = await GetYoutubeData(url);
            var id = (json["continuationContents"]?["liveChatContinuation"] ?? json["contents"]?["liveChatRenderer"])
                ?["header"]
                ?["liveChatHeaderRenderer"]
                ?["viewSelector"]
                ?["sortFilterSubMenuRenderer"]
                ?["subMenuItems"]
                ?.Last
                ?["continuation"]
                ?["reloadContinuationData"]
                ?["continuation"]?.Value<string>();
            return id == null ? null : new Uri($"{'h'}ttps://www.youtube.com/live_chat?continuation={id}");
        }
        private async Task<JArray> GetChatMessages(Uri url)
        {
            var json = await GetYoutubeData(url);
            if (json == null) return null;
            var jtok = (json["continuationContents"]?["liveChatContinuation"] ?? json["contents"]?["liveChatRenderer"])?["actions"];
            if (jtok == null || !(jtok is JArray jarr)) return null;
            return jarr;
        }
        private async Task<string> GetChannelStream(string channelID)
        {
            var json = await GetYoutubeData(YoutubeURL.GetChannel(channelID));
            foreach (JObject item in json?["contents"]?["twoColumnBrowseResultsRenderer"]?["tabs"]?[0]?["tabRenderer"]?["content"]?["sectionListRenderer"]?["contents"]?[0]?["itemSectionRenderer"]?["contents"]?[0]?["channelFeaturedContentRenderer"]?["items"] ?? new JArray())
                if (item?["videoRenderer"]?["thumbnailOverlays"]?[0]?["thumbnailOverlayTimeStatusRenderer"]?["style"]?.Value<string>() == "LIVE")
                    return item["videoRenderer"]["videoId"].Value<string>();
            return null;
        }
        private async Task<DateTime> GetStartDate(string videoID)
        {
            var response = await GetYoutubeResponse(YoutubeURL.GetVideoInfo(videoID));
            if (response.StatusCode != 200) return DateTime.MinValue;
            var data = HttpUtility.UrlDecode(Encoding.Default.GetString(response.Data));
            var json = JObject.Parse(HttpUtility.ParseQueryString(data).Get("player_response"));
            var start = json?["microformat"]?["playerMicroformatRenderer"]?["liveBroadcastDetails"]?["startTimestamp"]?.Value<string>();
            if (start == null) return DateTime.MinValue;
            start = start.Replace(' ', '+');
            return DateTime.Parse(start).ToUniversalTime();
        }
        private async Task<(string ChannelID, string Title, string Icon)> GetChannelData(Uri url)
        {
            try
            {
                var json = await GetYoutubeData(url);
                json = (JObject)json["metadata"]["channelMetadataRenderer"];
                // var _json = json.ToString();
                return (json["externalId"].Value<string>(), json["title"].Value<string>(), json["avatar"]?["thumbnails"]?[0]?["url"]?.Value<string>());
            }
            catch
            {
                return default;
            }
        }
        private async Task RunTask(TimeSpan updateTimeout, string channelId, CancellationToken token)
        {
            try
            {
                var videoId = await GetChannelStream(channelId);
                
                if (videoId == null)
                {
                    OnStatusReceived?.Invoke(new StatusResponse(404, "Stream not founded"));
                    return;
                }
                
                var startTime = await GetStartDate(videoId);
                
                if (startTime == DateTime.MinValue)
                {
                    OnStatusReceived?.Invoke(new StatusResponse(404, "Video info not founded"));
                    return;
                }
                
                var liveChatUrl = YoutubeURL.GetLiveChat(videoId);
                liveChatUrl = await GetFullLiveChat(liveChatUrl);
                
                if (liveChatUrl == null)
                {
                    OnStatusReceived?.Invoke(new StatusResponse(404, "LiveChat not founded"));
                    return;
                }
                
                string lastMessageId = null;
                var errors = 0;
                var utcInit = DateTime.UtcNow;
                var user = await GetChannelData(YoutubeURL.GetChannel(channelId));
                OnStatusReceived?.Invoke(new StatusResponse(200, $"LiveChat started: {user.Title}"));
                
                while (errors < 10)
                {
                    token.ThrowIfCancellationRequested();
                    
                    await Task.Delay(updateTimeout);
                    token.ThrowIfCancellationRequested();
                    string firstMessageId = null;
                    var messageResponses = new List<YTMessageResponse>();
                    var chatMessages = await GetChatMessages(liveChatUrl);
                    
                    if (chatMessages == null)
                    {
                        errors++;
                        continue;
                    }
                    
                    foreach (var jToken in chatMessages.Reverse())
                    {
                        var item = (JObject) jToken;
                        try
                        {
                            if (!item.TryGetValue("addChatItemAction", out var chatItem)) continue;
                            
                            chatItem = chatItem["item"]["liveChatTextMessageRenderer"];
                            
                            if (chatItem == null) continue;

                            var message = chatItem["message"]["runs"][0]["text"].Value<string>();
                            var authorName = chatItem["authorName"]?["simpleText"]?.Value<string>() ?? "Unknown";
                            var authorIcon = chatItem["authorPhoto"]?["thumbnails"]?.Last?["url"]?.Value<string>();
                            var authorID = chatItem["authorExternalChannelId"].Value<string>();
                            var messageID = chatItem["id"].Value<string>();
                            var value = chatItem["timestampUsec"].Value<long>() / 1000;
                            var utcTime = DateTimeOffset.FromUnixTimeMilliseconds(value).UtcDateTime;
                            var authorType = AuthorTypes.None;
                            foreach (var jbd in chatItem["authorBadges"]?.ToObject<JArray>() ?? new JArray())
                            {
                                var jjbd = jbd["liveChatAuthorBadgeRenderer"]?.ToObject<JObject>();
                                if (jjbd != null && jjbd.TryGetValue("icon", out var icon))
                                {
                                    switch (icon?["iconType"]?.Value<string>())
                                    {
                                        case "VERIFIED": authorType |= AuthorTypes.Verified; break;
                                        case "MODERATOR": authorType |= AuthorTypes.Moderator; break;
                                        case "OWNER": authorType |= AuthorTypes.Owner; break;
                                        default: authorType |= AuthorTypes.Other; break;
                                    }
                                }
                                else authorType |= AuthorTypes.Sponsor;
                            }
                            if (lastMessageId == null && utcTime < utcInit) continue;
                            if (firstMessageId == null) firstMessageId = messageID;
                            if (lastMessageId == messageID) break;
                            messageResponses.Add(new YTMessageResponse(messageID, authorName, authorID, message, utcTime, startTime, authorType,null));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }
                    messageResponses.Reverse();
                    foreach (var messageResponse in messageResponses)
                    {
                        token.ThrowIfCancellationRequested();
                        //Console.WriteLine($"{msg.UserName}: {msg.Context}");
                        OnMessageReceived?.Invoke(messageResponse);
                    }
                    errors = 0;
                    lastMessageId = firstMessageId ?? lastMessageId;
                    token.ThrowIfCancellationRequested();
                }
                
                if (errors > 10)
                    OnStatusReceived?.Invoke(new StatusResponse(400, "LiveChat closed"));
            }
            catch (Exception e)
            {
                OnStatusReceived?.Invoke(new StatusResponse(401, e.ToString()));
            }
        }
        
        public void Dispose()
        {
            if (_canceller == null) return;
            _canceller.Cancel();
            _canceller = null;
        }
    }
}
