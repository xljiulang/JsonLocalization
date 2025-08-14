using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace JsonLocalization
{
    sealed class LocalizerResourcePersisterService<TOptions> : BackgroundService
    {
        private readonly LocalizerResourcePersister<TOptions> resourcePersister;

        public LocalizerResourcePersisterService(LocalizerResourcePersister<TOptions> resourcePersister)
        {
            this.resourcePersister = resourcePersister;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return this.resourcePersister.WriteToFilesAsync(stoppingToken);
        }
    }
}
