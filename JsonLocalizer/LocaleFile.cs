using System.Collections.Generic;

namespace JsonLocalization
{
    sealed class LocaleFile<TLocale>
    {
        public required Dictionary<string, TLocale> Locales { get; init; }
    }
}
