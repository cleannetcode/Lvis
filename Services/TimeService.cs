using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Services
{
    class TimeService
    {
        string culture;
        public TimeService(ConfigurationService confService)
        {
            culture = confService.DateTimeFormat;
        }
        public string FormatActualTime => ActualTime.ToString(culture);
        public DateTime ActualTime => DateTime.UtcNow;
    }
}
