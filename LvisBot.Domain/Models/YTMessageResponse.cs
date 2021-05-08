using System;

namespace LvisBot.Domain.Models
{
    public class YTMessageResponse
    {
        public string MessageId { get; }
        public string UserName { get; }
        public string UserId { get; }
        public string Context { get; }
        public DateTime UtcTime { get; }
        public DateTime StartStreamTime { get; }
        public AuthorTypes UserType { get; }

        [Flags] 
        public enum AuthorTypes
        {
            None = 0,
            Other = 1 << 0,
            Verified = 1 << 1,
            Owner = 1 << 2,
            Moderator = 1 << 3,
            Sponsor = 1 << 4,
        }

        public YTMessageResponse(string messageId, string userName, string userId, string context, DateTime utcTime, DateTime startStreamTime, AuthorTypes userType)
        {
            MessageId = messageId;
            UserName = userName;
            UserId = userId;
            Context = context;
            UtcTime = utcTime;
            UserType = userType;
            StartStreamTime = startStreamTime;
        }
    }
}
