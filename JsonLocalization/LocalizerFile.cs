using System.Collections.Generic;

namespace JsonLocalization
{
    sealed class LocalizerFile<TOptions>
    {
        public required Dictionary<string, TOptions> Cultures { get; init; }
    }
}
