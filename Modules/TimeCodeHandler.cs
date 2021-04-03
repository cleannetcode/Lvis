using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Models;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Services;

namespace YouTubeChatBot.Modules
{
    class TimeCodeHandler : IActionModule<YTMessageResponse>
    {
        public TimeCodeHandler(FileService fileService, SerializeService serializeService)
        {

        }
        public void Execute(YTMessageResponse param)
        {
            throw new NotImplementedException();
        }
    }
}
