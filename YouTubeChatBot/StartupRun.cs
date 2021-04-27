using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeChatBot
{
    partial class Startup : IDisposable
    {
        DICargo di;
        public Task RunAsync()
        {
            return Task.Run(Run);
        }
        public void Run()
        {
            di = new DICargo();
            ConfigureServices(di);
            RunWith(di);
        }

        public void Dispose()
        {
            di.Dispose();
        }
    }
}
