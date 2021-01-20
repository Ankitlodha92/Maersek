using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaersekSorting
{
    public interface IWorker
    {
       public  Task  DoWork(CancellationToken cancellationToken);
    }
}
