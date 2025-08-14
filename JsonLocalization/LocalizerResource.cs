using System.Collections.Generic;

namespace JsonLocalization
{
    sealed class LocalizerResource<TOptions>
    {
        public required Dictionary<string, TOptions> Cultures { get; init; }
    }
}
