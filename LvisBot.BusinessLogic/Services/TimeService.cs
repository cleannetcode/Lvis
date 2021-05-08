using System;

namespace LvisBot.BusinessLogic.Services
{
    public class TimeService
    {
        private readonly string _culture;
        
        public TimeService(ConfigurationService confService)
        {
            _culture = confService.DateTimeFormat;
        }
        
        public string FormatActualTime => ActualTime.ToString(_culture);
        
        public DateTime ActualTime => DateTime.UtcNow;
    }
}
