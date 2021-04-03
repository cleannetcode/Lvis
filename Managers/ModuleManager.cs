using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Interfaces;
using System.Threading.Tasks;

namespace YouTubeChatBot.Managers
{
    abstract class ModuleManager<TPrefix, TMessMod, TStatMod, TListenerConf> : IDisposable
    {
        Dictionary<TPrefix, (IActionModule<TMessMod> Item, Func<IActionModule<TMessMod>> Build)> actionModules;
        ISourceListener<TListenerConf, TMessMod, TStatMod>[] sourceListeners;
        public ModuleManager(params ISourceListener<TListenerConf, TMessMod, TStatMod>[] listeners)
        {
            if (listeners == null || listeners.Length <= 0)
            {
                throw new ArgumentException($"{nameof(listeners)} cant be null or empty");
            }
            ListenersManage(l => 
            {
                l.MessageEvent += MessageUpdate;
                l.StatusEvent += (e) => StateHandle(l, e);
            });
        }
        private void ListenersManage(Action<ISourceListener<TListenerConf, TMessMod, TStatMod>> listenerAction)
        {
            foreach (var item in sourceListeners)
            {
                listenerAction(item);
            }
        }
        private void MessageUpdate(TMessMod mess)
        {
            var prefix = GetPrefix(mess);
            if (prefix == null) return;
            if (actionModules.TryGetValue(prefix, out var module))
            {
                if (module.Item == null)
                {
                    module.Item = module.Build();
                }
                module.Item.Execute(mess);
            }
        }
        public void AddModule<T>(TPrefix commandPrefix) where T : IActionModule<TMessMod>, new() => AddModule(commandPrefix, () => new T());
        public void AddModule(TPrefix commandPrefix, Func<IActionModule<TMessMod>> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (actionModules.ContainsKey(commandPrefix))
            {
                throw new ArgumentException($"{commandPrefix} has already been added");
            }
            actionModules.Add(commandPrefix, (null, builder));
        }
        public void Run(TListenerConf config)
        {
            ListenersManage(l => Task.Run(() => l.Run(config)));
        }
        public void Dispose()
        {
            ListenersManage(l =>
            {
                l.MessageEvent -= MessageUpdate;
                l.StatusEvent -= (e) => StateHandle(l, e);
                l.Dispose();
            });
            foreach (var item in actionModules)
            {
                (item as IDisposable)?.Dispose();
            }
            actionModules = null;
            sourceListeners = null;
        }
        protected abstract TPrefix GetPrefix(TMessMod mess);
        protected abstract void StateHandle(ISourceListener<TListenerConf, TMessMod, TStatMod> sender, TStatMod StatusModel);
    }
}
