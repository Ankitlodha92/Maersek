using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaersekSorting
{
    public class DeriverdBackground :BackgroundService
    {
        private readonly IWorker _worker;

        public DeriverdBackground(IWorker worker)
        {
            _worker = worker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           await _worker.DoWork(stoppingToken);
        }
    }
}
