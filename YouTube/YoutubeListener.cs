using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Models;
using YouTubeChatBot.Services;

namespace YouTubeChatBot.YouTube
{
    class YoutubeListener : ISourceListener<YouTubeConfig, YTMessageResponse, StatusResponse>
    {
        private static class YoutubeURL
        {
            public static Uri GetVideoInfo(string videoID) => new Uri($"{'h'}ttps://www.youtube.com/get_video_info?video_id={videoID}");
            public static Uri GetLiveChat(string videoID) => new Uri($"{'h'}ttps://www.youtube.com/live_chat?v={videoID}");
            public static Uri GetChannel(string channelID) => new Uri($"{'h'}ttps://www.youtube.com/channel/{channelID}");
        }

        private readonly NetService service;
        private CancellationTokenSource canceller;

        public YoutubeListener(NetService service)
        {
            this.service = service;
        }

        public event Action<YTMessageResponse> MessageEvent;
        public event Action<StatusResponse> StatusEvent;

        public void Dispose()
        {
            if (canceller == null) return;
            canceller.Cancel();
            canceller = null;
        }
        public void Run(YouTubeConfig configuration)
        {
            if (canceller != null) canceller.Cancel();
            canceller = new CancellationTokenSource();
            _ = RunTask(TimeSpan.FromMilliseconds(configuration.UpdateMs), configuration.ChannelID, canceller.Token);
        }

        private async Task<NetResponse> GetYoutubeResponse(Uri url) => await service.Request(url, NetService.RequestMethod.GET, new Dictionary<string, string>()
        {
            ["useragent"] = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
        });
        private async Task<JObject> GetYoutubeData(Uri url)
        {
            const string begin = "window[\"ytInitialData\"] = ";
            const string end = ";</script>";
            NetResponse response = await GetYoutubeResponse(url);
            if (response.StatusCode != 200) return null;
            string html = Encoding.Default.GetString(response.Data);
            return JObject.Parse(html[(html.IndexOf(begin) + begin.Length)..html.IndexOf(end)]);
        }
        private async Task<Uri> GetFullLiveChat(Uri url)
        {
            JObject json = await GetYoutubeData(url);
            string id = (json["continuationContents"]?["liveChatContinuation"] ?? json["contents"]?["liveChatRenderer"])
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
            JObject json = await GetYoutubeData(url);
            if (json == null) return null;
            JToken jtok = (json["continuationContents"]?["liveChatContinuation"] ?? json["contents"]?["liveChatRenderer"])?["actions"];
            if (jtok == null || !(jtok is JArray jarr)) return null;
            return jarr;
        }
        private async Task<string> GetChannelStream(string channelID)
        {
            JObject json = await GetYoutubeData(YoutubeURL.GetChannel(channelID));
            foreach (JObject item in json?["contents"]?["twoColumnBrowseResultsRenderer"]?["tabs"]?[0]?["tabRenderer"]?["content"]?["sectionListRenderer"]?["contents"]?[0]?["itemSectionRenderer"]?["contents"]?[0]?["channelFeaturedContentRenderer"]?["items"] ?? new JArray())
                if (item?["videoRenderer"]?["thumbnailOverlays"]?[0]?["thumbnailOverlayTimeStatusRenderer"]?["style"]?.Value<string>() == "LIVE")
                    return item["videoRenderer"]["videoId"].Value<string>();
            return null;
        }
        private async Task<DateTime> GetStartDate(string videoID)
        {
            NetResponse response = await GetYoutubeResponse(YoutubeURL.GetVideoInfo(videoID));
            if (response.StatusCode != 200) return DateTime.MinValue;
            string data = HttpUtility.UrlDecode(Encoding.Default.GetString(response.Data));
            JObject json = JObject.Parse(HttpUtility.ParseQueryString(data).Get("player_response"));
            string start = json?["microformat"]?["playerMicroformatRenderer"]?["liveBroadcastDetails"]?["startTimestamp"]?.Value<string>();
            if (start == null) return DateTime.MinValue;
            return DateTime.Parse(start).ToUniversalTime();
        }
        private async Task RunTask(TimeSpan updateTimeout, string channelID, CancellationToken token)
        {
            string videoID = await GetChannelStream(channelID);
            if (videoID == null)
            {
                StatusEvent?.Invoke(new StatusResponse(404, "Stream not founded"));
                return;
            }
            DateTime startTime = await GetStartDate(videoID);
            if (startTime == DateTime.MinValue)
            {
                StatusEvent?.Invoke(new StatusResponse(404, "Video info not founded"));
                return;
            }
            Uri liveChatUrl = YoutubeURL.GetLiveChat(videoID);
            liveChatUrl = await GetFullLiveChat(liveChatUrl);
            if (liveChatUrl == null)
            {
                StatusEvent?.Invoke(new StatusResponse(404, "LiveChat not founded"));
                return;
            }
            string lastMessageID = null;
            int errors = 0;
            DateTime utcInit = DateTime.UtcNow;
            StatusEvent?.Invoke(new StatusResponse(200, "LiveChat starded"));
            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (errors > 10)
                {
                    StatusEvent?.Invoke(new StatusResponse(400, "LiveChat closed"));
                    return;
                }
                await Task.Delay(updateTimeout);
                token.ThrowIfCancellationRequested();
                string firstMessageID = null;
                List<YTMessageResponse> msgs = new List<YTMessageResponse>();
                JArray chatMessages = await GetChatMessages(liveChatUrl);
                if (chatMessages == null)
                {
                    errors++;
                    continue;
                }
                foreach (JObject item in chatMessages.Reverse())
                {
                    try
                    {
                        if (!item.TryGetValue("addChatItemAction", out JToken chatItem)) continue;
                        chatItem = chatItem["item"]["liveChatTextMessageRenderer"];
                        if (chatItem == null) continue;

                        string message = chatItem["message"]["runs"][0]["text"].Value<string>();
                        string authorName = chatItem["authorName"]?["simpleText"]?.Value<string>() ?? "Unknown";
                        string authorIcon = chatItem["authorPhoto"]?["thumbnails"]?.Last?["url"]?.Value<string>();
                        string authorID = chatItem["authorExternalChannelId"].Value<string>();
                        string messageID = chatItem["id"].Value<string>();
                        long value = chatItem["timestampUsec"].Value<long>() / 1000;
                        DateTime utcTime = DateTimeOffset.FromUnixTimeMilliseconds(value).UtcDateTime;
                        YTMessageResponse.AuthorTypes authorType = YTMessageResponse.AuthorTypes.None;
                        foreach (var jbd in chatItem["authorBadges"]?.ToObject<JArray>() ?? new JArray())
                        {
                            JObject jjbd = jbd["liveChatAuthorBadgeRenderer"]?.ToObject<JObject>();
                            if (jjbd != null && jjbd.TryGetValue("icon", out JToken icon))
                            {
                                switch (icon?["iconType"]?.Value<string>())
                                {
                                    case "VERIFIED": authorType |= YTMessageResponse.AuthorTypes.Verified; break;
                                    case "MODERATOR": authorType |= YTMessageResponse.AuthorTypes.Moderator; break;
                                    case "OWNER": authorType |= YTMessageResponse.AuthorTypes.Owner; break;
                                    default: authorType |= YTMessageResponse.AuthorTypes.Other; break;
                                }
                            }
                            else authorType |= YTMessageResponse.AuthorTypes.Sponsor;
                        }
                        if (lastMessageID == null && utcTime < utcInit) continue;
                        if (firstMessageID == null) firstMessageID = messageID;
                        if (lastMessageID == messageID) break;
                        msgs.Add(new YTMessageResponse(authorName, authorID, message, utcTime, startTime, authorType));
                    }
                    catch
                    {

                    }
                }
                msgs.Reverse();
                foreach (var msg in msgs)
                {
                    token.ThrowIfCancellationRequested();
                    MessageEvent?.Invoke(msg);
                }
                errors = 0;
                lastMessageID = firstMessageID ?? lastMessageID;
                token.ThrowIfCancellationRequested();
            }
        }
    }
}




































