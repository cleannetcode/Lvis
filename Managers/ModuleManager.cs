using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Interfaces;

namespace YouTubeChatBot.Managers
{
    abstract class ModuleManager<TMessMod, TStatMod>
    {
        public ModuleManager()
        {

        }
        public void AddRequestModule<T>(string commandPrefix) where T : IActionModule<TMessMod>, new() => AddRequestModule(commandPrefix, () => new T());
        public void AddRequestModule<T>(string commandPrefix, Func<T> builder) where T : IActionModule<TMessMod>
        {

        }
        public void AddStateModule<T>(string commandPrefix) where T : IActionModule<TStatMod>, new() => AddStateModule(commandPrefix, () => new T());
        public void AddStateModule<T>(string commandPrefix, Func<T> builder) where T : IActionModule<TStatMod>
        {

        }
        protected abstract void StateHandle(TStatMod StatusModel);
    }
}
