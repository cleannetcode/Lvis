using System;

namespace LvisBot.Domain.Models
{
    public class YTVideoDataResponse
    {
        public string VideoTitle { get; }
        public DateTime VideoPublishedAt { get; }
        public string IdLiveChat { get; }
        public ulong? VideoCountLikes { get; }
        public ulong? VideoCountView { get; } 
        public YTVideoDataResponse(
            string videoTitle, 
            DateTime videoPublishedAt,
            string idLiveChat,
            ulong? videoCountLikes,
            ulong? videoCountView)
        {
            VideoTitle = videoTitle;
            VideoPublishedAt = videoPublishedAt;
            IdLiveChat = idLiveChat;
            VideoCountLikes = videoCountLikes;
            VideoCountView = videoCountView;
        }
    }
}