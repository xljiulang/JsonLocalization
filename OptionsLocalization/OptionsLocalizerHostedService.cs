using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OptionsLocalization
{
    sealed class OptionsLocalizerHostedService : BackgroundService
    {
        private readonly IEnumerable<IOptionsLocalizer> optionsLocalizers;

        public OptionsLocalizerHostedService(IEnumerable<IOptionsLocalizer> optionsLocalizers)
        {
            this.optionsLocalizers = optionsLocalizers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            foreach (var optionsLocalizer in this.optionsLocalizers.Distinct())
            {
                optionsLocalizer.WriteToValueFiles();
            }
        }
    }
}
