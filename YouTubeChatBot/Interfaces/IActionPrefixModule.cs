using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Interfaces
{
    interface IActionPrefixModule<TMess,TPrefix> : IActionModule<TMess>
    {
        TPrefix Prefix { get; }
    }
}
