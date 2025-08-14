using Microsoft.Extensions.Options;
using System.Threading;

namespace JsonLocalization
{
    sealed class Localizer<TOptions> : ILocalizer<TOptions>
    {
        private readonly IOptionsMonitor<TOptions> optionsMonitor;

        public TOptions Current => this.Get(Thread.CurrentThread.CurrentCulture.Name);

        public TOptions Get(string culture) => this.optionsMonitor.Get(culture);

        public Localizer(IOptionsMonitor<TOptions> optionsMonitor)
        {
            this.optionsMonitor = optionsMonitor;
        }
    }
}
