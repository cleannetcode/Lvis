using System;
using System.Threading;
using System.Threading.Tasks;
using LvisBot.Domain.Models;

namespace LvisBot.Domain.Interfaces
{
    //impl: TConf - YouTubeConf, TMess - MessageResponse
    public interface ISourceListener<TConf, TMess, TStat> : IDisposable
    {
        event Action<TMess> OnMessageReceived;
        event Action<TStat> OnStatusReceived;
        void Run(TConf configuration);
        Task RunAsync(TConf configuration, CancellationToken token);
    }
}
