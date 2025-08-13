using System.Collections.Generic;

namespace JsonLocalizer
{
    sealed class LocaleFile<TLocale>
    {
        public required Dictionary<string, TLocale> Locales { get; init; }
    }
}
