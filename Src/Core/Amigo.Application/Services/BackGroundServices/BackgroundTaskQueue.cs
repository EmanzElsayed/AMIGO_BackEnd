using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Amigo.Application.Services.BackGroundServices
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<
            Func<CancellationToken, IServiceProvider, Task>
        > _queue =
            Channel.CreateUnbounded<
                Func<CancellationToken, IServiceProvider, Task>
            >();

        public void QueueTask(
            Func<CancellationToken, IServiceProvider, Task> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            _queue.Writer.TryWrite(task);
        }

        public async Task<
            Func<CancellationToken, IServiceProvider, Task>
        > DequeueAsync(CancellationToken token)
        {
            return await _queue.Reader.ReadAsync(token);
        }
    }
}