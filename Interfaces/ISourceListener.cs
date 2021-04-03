using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Models;

namespace YouTubeChatBot.Interfaces
{
    //impl: Tconf - YouTubeConf, TMess - MessageResponse
    interface ISourceListener<TConf, TMess> : IDisposable
    {
        event Action<TMess> MessageEvent;
        void Run(TConf configuration);
    }
}
