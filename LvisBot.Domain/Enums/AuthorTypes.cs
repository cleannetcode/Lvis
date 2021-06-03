using System;

namespace LvisBot.Domain.Models
{
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
}