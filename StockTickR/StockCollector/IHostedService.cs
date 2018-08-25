using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting {
    //
    // Summary:
    //     Defines methods for objects that are managed by the host.
    public interface IHostedService {
        // Summary:
        // Triggered when the application host is ready to start the service.
        Task StartAsync (CancellationToken stoppingToken);

        // Summary:
        // Triggered when the application host is performing a graceful shutdown.
        Task StopAsync (CancellationToken stoppingToken);
    }
}