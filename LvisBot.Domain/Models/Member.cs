using System;
using LvisBot.Domain.Models;

namespace LvisBot.Domain.Models
{
    public class Member
    {
        public string UserName { get; set; }
        public DateTime DateRegistration { get; set; }
        public string FirstMessage { get; set; }
        public string ChannelId { get; set; }
        public string ProfileImageUrl  { get; set; }
        public AuthorTypes UserType { get; set; }
    }
}