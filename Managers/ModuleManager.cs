using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Interfaces;

namespace YouTubeChatBot.Managers
{
    class ModuleManager<TModule>
    {
        public ModuleManager()
        {

        }
        public void AddModule<T>(string commandPrefix) where T: IActionModule<TModule>
        {

        }
        public void AddModule<T>(string commandPrefix, Func<T> builder) where T : IActionModule<TModule>
        {

        }
    }
}
