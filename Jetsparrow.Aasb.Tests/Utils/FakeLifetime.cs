using System.Threading;

namespace Jetsparrow.Aasb.Tests.Utils;
public class FakeLifetime : IHostApplicationLifetime
{
    CancellationTokenSource Started = new(), Stopping = new(), Stopped = new();

    public CancellationToken ApplicationStarted => Started.Token;

    public CancellationToken ApplicationStopping => Stopping.Token;

    public CancellationToken ApplicationStopped => Stopped.Token;

    public void StopApplication()
    {
        Stopping.Cancel();
    }
}
