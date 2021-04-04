using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Models;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Services;
using System.IO;

namespace YouTubeChatBot.Modules
{
    class TimeCodeHandler : BaseYTModule
    {
        protected override string savefileformat => "Stream[{0}]TimeCodes";
        public TimeCodeHandler(FileService fileService, SerializeService serializeService, ConfigurationService configurationService)
            : base(fileService, serializeService, configurationService.TimeCodeConfigure)
        {

        }
    }
}
