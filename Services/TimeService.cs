using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Services
{
    class TimeService
    {
        public TimeService(ConfigurationService confService)
        {

        }
        public string FormatActualTime { get; }
        public DateTime ActualTime { get; }
    }
}
