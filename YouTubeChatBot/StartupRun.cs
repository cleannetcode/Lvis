using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LvisBot.CargoDI;

namespace YouTubeChatBot
{
    partial class Startup : IDisposable
    {
        CargoCollection _cargoCollection;
        public Task RunAsync()
        {
            return Task.Run(Run);
        }
        public void Run()
        {
            _cargoCollection = new CargoCollection();
            ConfigureServices(_cargoCollection);
            RunWith(_cargoCollection);
        }

        public void Dispose()
        {
            _cargoCollection.Dispose();
        }
    }
}
