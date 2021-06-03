using System;

namespace LvisBot.Domain.Models
{
    public class Member
    {
        public string UserName { get; set; }
        public DateTime DateRegistration { get; set; }
        public string FirstMessage { get; set; }
    }
}