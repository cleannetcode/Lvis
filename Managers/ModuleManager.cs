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
        List<ISourceListener<TListenerConf, TMessMod, TStatMod>> sourceListeners;
        public ModuleManager()
        {
            actionModules = new Dictionary<TPrefix, (IActionModule<TMessMod> Item, Func<IActionModule<TMessMod>> Build)>();
            sourceListeners = new List<ISourceListener<TListenerConf, TMessMod, TStatMod>>();
        }
        public ModuleManager(Action<ModuleManager<TPrefix, TMessMod, TStatMod, TListenerConf>> configure) : this()
        {
            configure?.Invoke(this);
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
        public void AddListener<T>() where T : ISourceListener<TListenerConf, TMessMod, TStatMod>, new() => AddListener(() => new T());
        public void AddListener(Func<ISourceListener<TListenerConf, TMessMod, TStatMod>> listenerBuilder)
        {
            if (listenerBuilder == null)
            {
                throw new ArgumentNullException(nameof(listenerBuilder));
            }
            AddListener(listenerBuilder());
        }
        public void AddListener(ISourceListener<TListenerConf, TMessMod, TStatMod> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            sourceListeners.Add(listener);
            listener.MessageEvent += MessageUpdate;
            listener.StatusEvent += (e) => StateHandle(listener, e);
        }
        public void AddModule<T>() where T : IActionPrefixModule<TMessMod, TPrefix>, new() => AddModule(() => new T());
        public void AddModule(Func<IActionPrefixModule<TMessMod, TPrefix>> builder)
        {
            var item = builder();
            AddModule(item.Prefix, () => item);
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
                //l.Dispose();
            });
            //foreach (var item in actionModules)
            //{
            //    (item as IDisposable)?.Dispose();
            //}
            actionModules = null;
            sourceListeners = null;
        }
        protected abstract TPrefix GetPrefix(TMessMod mess);
        protected abstract void StateHandle(ISourceListener<TListenerConf, TMessMod, TStatMod> sender, TStatMod StatusModel);
    }
}
