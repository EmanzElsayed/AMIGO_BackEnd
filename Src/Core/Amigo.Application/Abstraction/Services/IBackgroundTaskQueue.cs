using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Application.Abstraction.Services
{
    public interface IBackgroundTaskQueue
    {
        void QueueTask(Func<CancellationToken, IServiceProvider, Task> task);
        Task<Func<CancellationToken, IServiceProvider, Task>> DequeueAsync(CancellationToken token);
    }
}
