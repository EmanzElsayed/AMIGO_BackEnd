using System.Threading.Channels;

namespace Amigo.Application.BackgroundTasks
{
    public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
    {
    private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue;

   
    public BackgroundTaskQueue(int capacity = 200)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,   // only the hosted service reads
            SingleWriter = false   // multiple request threads can write
        };

        _queue = Channel.CreateBounded<Func<IServiceProvider, CancellationToken, Task>>(options);
    }

    public async ValueTask EnqueueAsync(
        Func<IServiceProvider, CancellationToken, Task> workItem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workItem);
        await _queue.Writer.WriteAsync(workItem, cancellationToken);
    }

    public async ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken) =>
        await _queue.Reader.ReadAsync(cancellationToken);
}
}