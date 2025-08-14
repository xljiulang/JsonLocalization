using Microsoft.Extensions.Options;
using System.Threading;

namespace JsonLocalization
{
    sealed class Localizer<TModel> : ILocalizer<TModel>
    {
        private readonly IOptionsMonitor<TModel> modelFactory;

        public TModel Current => this.Get(Thread.CurrentThread.CurrentCulture.Name);

        public TModel Get(string culture) => this.modelFactory.Get(culture);

        public Localizer(IOptionsMonitor<TModel> modelFactory)
        {
            this.modelFactory = modelFactory;
        }
    }
}
