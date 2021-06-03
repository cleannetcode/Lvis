using System;

namespace LvisBot.Domain.Models
{
    public partial class YTMessageResponse
    {
        public string MessageId { get; }
        public string UserName { get; }
        public string ChannelId { get; }
        public string Context { get; }
        public DateTime UtcTime { get; }
        public DateTime StartStreamTime { get; }
        public AuthorTypes UserType { get; }
        public string ProfileImageUrl { get; set; }

        public YTMessageResponse(
            string messageId, 
            string userName, 
            string channelId, 
            string context, 
            DateTime utcTime, 
            DateTime startStreamTime, 
            AuthorTypes userType,
            string profileImageUrl)
        {
            MessageId = messageId;
            UserName = userName;
            ChannelId = channelId;
            Context = context;
            UtcTime = utcTime;
            UserType = userType;
            StartStreamTime = startStreamTime;
            ProfileImageUrl = profileImageUrl;
        }
    }
}
