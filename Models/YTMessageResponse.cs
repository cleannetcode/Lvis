using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Models
{
    class YTMessageResponse
    {
        public string UserName { get; }
        public string UserID { get; }
        public string Context { get; }
        public DateTime UtcTime { get; }
        public AuthorTypes UserType { get; }

        [Flags] public enum AuthorTypes
        {
            None = 0,
            Other = 1 << 0,
            Verified = 1 << 1,
            Owner = 1 << 2,
            Moderator = 1 << 3,
            Sponsor = 1 << 4,
        }

        public YTMessageResponse(string userName, string userID, string context, DateTime utcTime, AuthorTypes userType)
        {
            UserName = userName;
            UserID = userID;
            Context = context;
            UtcTime = utcTime;
            UserType = userType;
        }
    }
}
