using System.Collections.Generic;
using System.Globalization;

namespace OptionsLocalization
{
    sealed class OptionsLocalizerOptions<TOptions>
    {
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;
        public string? OptionsPath { get; set; }
        public IReadOnlyCollection<CultureInfo> OptionsCultures { get; set; } = [];
    }
}
