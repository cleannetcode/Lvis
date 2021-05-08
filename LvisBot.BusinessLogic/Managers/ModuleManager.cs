using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LvisBot.Domain.Interfaces;

namespace LvisBot.BusinessLogic.Managers
{
    public abstract class ModuleManager<TPrefix, TMessMod, TStatMod, TListenerConf> : IDisposable
    {
        private Dictionary<TPrefix, (IActionModule<TMessMod> Item, Func<IActionModule<TMessMod>> Build)> _actionModules;
        private List<ISourceListener<TListenerConf, TMessMod, TStatMod>> _sourceListeners;

        protected ModuleManager()
        {
            _actionModules = new Dictionary<TPrefix, (IActionModule<TMessMod> Item, Func<IActionModule<TMessMod>> Build)>();
            _sourceListeners = new List<ISourceListener<TListenerConf, TMessMod, TStatMod>>();
        }

        protected ModuleManager(Action<ModuleManager<TPrefix, TMessMod, TStatMod, TListenerConf>> configure) : this()
        {
            configure?.Invoke(this);
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
            _sourceListeners.Add(listener);
            listener.OnMessageReceived += OnMessageReceived;
            listener.OnStatusReceived += (e) => OnStatusReceived(listener, e);
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
            if (_actionModules.ContainsKey(commandPrefix))
            {
                throw new ArgumentException($"{commandPrefix} has already been added");
            }
            _actionModules.Add(commandPrefix, (null, builder));
        }
        
        public void Run(TListenerConf config)
        {
            Task.WaitAll(_sourceListeners.Select(l => Task.Run(() => l.Run(config))).ToArray());
        }
        
        protected abstract TPrefix GetPrefix(TMessMod mess);
        protected abstract void OnStatusReceived(ISourceListener<TListenerConf, TMessMod, TStatMod> sender, TStatMod statusModel);
        
        private void OnMessageReceived(TMessMod mess)
        {
            var prefix = GetPrefix(mess);
            if (prefix == null) return;
            if (_actionModules.TryGetValue(prefix, out var module))
            {
                if (module.Item == null)
                {
                    module.Item = module.Build();
                }
                module.Item.Execute(mess);
            }
        }
        
        public void Dispose()
        {
            foreach (var l in _sourceListeners)
            {
                l.OnMessageReceived -= OnMessageReceived;
                l.OnStatusReceived -= (e) => OnStatusReceived(l, e);
            }
            
            _actionModules = null;
            _sourceListeners = null;
        }
    }
}
