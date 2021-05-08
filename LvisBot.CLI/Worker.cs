using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LvisBot.BusinessLogic.Managers;
using LvisBot.CargoDI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LvisBot.CLI
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly YTModuleManager _manager;

        public Worker(ILogger<Worker> logger, YTModuleManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _manager.RunAsync(stoppingToken);
            //
            // while (!stoppingToken.IsCancellationRequested)
            // {
            //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //     await Task.Delay(1000, stoppingToken);
            // }
        }
    }
}