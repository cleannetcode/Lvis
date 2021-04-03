using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Interfaces;
using System.Threading.Tasks;

namespace YouTubeChatBot.Managers
{
    abstract class ModuleManager<TPrefix, TMessMod, TStatMod, TListenerConf> : IDisposable
    {
        Dictionary<TPrefix, IActionModule<TMessMod>> actionModules;
        ISourceListener<TListenerConf, TMessMod, TStatMod>[] sourceListeners;
        Func<TMessMod, TPrefix> GetPrefix;
        public ModuleManager(Func<TMessMod, TPrefix> toPrefix, params ISourceListener<TListenerConf, TMessMod, TStatMod>[] listeners)
        {
            if (toPrefix == null)
            {
                throw new ArgumentNullException(nameof(toPrefix));
            }
            if (listeners == null || listeners.Length <= 0)
            {
                throw new ArgumentException($"{nameof(listeners)} cant be null or empty");
            }
            GetPrefix = toPrefix;
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
            if (actionModules.TryGetValue(prefix, out var module))
            {
                module.Execute(mess);
            }
            else
            {
                throw new InvalidOperationException($"{prefix} was not found");
            }
        }
        public void AddModule<T>(TPrefix commandPrefix) where T : IActionModule<TMessMod>, new() => AddModule(commandPrefix, () => new T());
        public void AddModule(TPrefix commandPrefix, Func<IActionModule<TMessMod>> builder)
        {

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
            GetPrefix = null;
        }

        protected abstract void StateHandle(ISourceListener<TListenerConf, TMessMod, TStatMod> sender, TStatMod StatusModel);
    }
}
