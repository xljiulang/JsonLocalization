using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace JsonLocalizer
{
    sealed class LocalePersisterService<TTLocale> : BackgroundService
    {
        private readonly LocalePersister<TTLocale> localePersister;

        public LocalePersisterService(LocalePersister<TTLocale> localePersister)
        {
            this.localePersister = localePersister;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return this.localePersister.SaveAsync(stoppingToken);
        }
    }
}
