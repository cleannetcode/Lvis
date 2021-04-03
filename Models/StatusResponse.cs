using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Models
{
    class StatusResponse
    {
        public int Code { get; }
        public string State { get; }
        public StatusResponse(int code, string state)
        {
            Code = code;
            State = state;
        }
    }
}
