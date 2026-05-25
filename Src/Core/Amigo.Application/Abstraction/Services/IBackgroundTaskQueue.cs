
using System.Threading.Channels;
namespace Amigo.Application.BackgroundTasks
{
    public interface IBackgroundTaskQueue
    {
        ValueTask EnqueueAsync(
            Func<IServiceProvider, CancellationToken, Task> workItem,
            CancellationToken cancellationToken = default);

        ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }


}