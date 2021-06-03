using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LvisBot.BusinessLogic.Services;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Managers
{
    public abstract class ModuleManager<TPrefix, TStatMod, TListenerConf> : IDisposable
    {
        private Dictionary<TPrefix, (IActionModule<YTMessageResponse> Item, Func<IActionModule<YTMessageResponse>> Build)> _actionModules;
        private List<ISourceListener<TListenerConf, YTMessageResponse, TStatMod>> _sourceListeners;
        private IMemberService _memberService;

        protected ModuleManager()
        {
            _actionModules = new Dictionary<TPrefix, (IActionModule<YTMessageResponse> Item, Func<IActionModule<YTMessageResponse>> Build)>();
            _sourceListeners = new List<ISourceListener<TListenerConf, YTMessageResponse, TStatMod>>();
        }

        protected ModuleManager(Action<ModuleManager<TPrefix, TStatMod, TListenerConf>> configure, IMemberService memberService) : this()
        {
            configure?.Invoke(this);
            _memberService = memberService;
        }
        
        public void AddListener<T>() where T : ISourceListener<TListenerConf, YTMessageResponse, TStatMod>, new() => AddListener(() => new T());
        
        public void AddListener(Func<ISourceListener<TListenerConf, YTMessageResponse, TStatMod>> listenerBuilder)
        {
            if (listenerBuilder == null)
            {
                throw new ArgumentNullException(nameof(listenerBuilder));
            }
            AddListener(listenerBuilder());
        }
        
        public void AddListener(ISourceListener<TListenerConf, YTMessageResponse, TStatMod> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }
            _sourceListeners.Add(listener);
            listener.OnMessageReceived += OnMessageReceived;
            listener.OnStatusReceived += (e) => OnStatusReceived(listener, e);
        }
        
        public void AddModule<T>() where T : IActionPrefixModule<YTMessageResponse, TPrefix>, new() => AddModule(() => new T());
        
        public void AddModule(Func<IActionPrefixModule<YTMessageResponse, TPrefix>> builder)
        {
            var item = builder();
            AddModule(item.Prefix, () => item);
        }
        
        public void AddModule<T>(TPrefix commandPrefix) where T : IActionModule<YTMessageResponse>, new() => AddModule(commandPrefix, () => new T());
        
        public void AddModule(TPrefix commandPrefix, Func<IActionModule<YTMessageResponse>> builder)
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
            var tokenSource = new CancellationTokenSource();

            var listeners = _sourceListeners
                .Select(l => l.RunAsync(config, tokenSource.Token))
                .ToArray();
            
            Task.WaitAll(listeners, tokenSource.Token);
        }
        
        protected abstract TPrefix GetPrefix(YTMessageResponse mess);
        protected abstract void OnStatusReceived(ISourceListener<TListenerConf, YTMessageResponse, TStatMod> sender, TStatMod statusModel);
        
        private void OnMessageReceived(YTMessageResponse mess)
        {
            _memberService.ChechUniqueMember(new Member()
            {
                UserName = mess.UserName,
                FirstMessage = mess.Context,
                DateRegistration = mess.UtcTime,
                ChannelId = mess.ChannelId,
                UserType = mess.UserType,
                ProfileImageUrl = mess.ProfileImageUrl
            });
            
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
