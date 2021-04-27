using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Models;

namespace YouTubeChatBot.Interfaces
{
    //impl: TConf - YouTubeConf, TMess - MessageResponse
    interface ISourceListener<TConf, TMess, TStat> : IDisposable
    {
        event Action<TMess> MessageEvent;
        event Action<TStat> StatusEvent;
        void Run(TConf configuration);
    }
}
