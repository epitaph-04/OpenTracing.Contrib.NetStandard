using System.Threading;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.NetStandard
{
	public interface ITracingService
	{
		Task StartAsync(CancellationToken cancellationToken);
		Task StopAsync(CancellationToken cancellationToken);
	}
}