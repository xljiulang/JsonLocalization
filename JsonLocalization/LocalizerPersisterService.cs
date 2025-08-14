using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace JsonLocalization
{
    sealed class LocalizerPersisterService<TTLocale> : BackgroundService
    {
        private readonly LocalizerPersister<TTLocale> localePersister;

        public LocalizerPersisterService(LocalizerPersister<TTLocale> localePersister)
        {
            this.localePersister = localePersister;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return this.localePersister.SaveAsync(stoppingToken);
        }
    }
}
