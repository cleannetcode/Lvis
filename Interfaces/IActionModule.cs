using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Interfaces
{
    //TMess - MessageResponse
    interface IActionModule<TMess>
    {
        void Execute(TMess param);
    }
}
