using System.Collections.Generic;
using System.Globalization;

namespace OptionsLocalization
{
    sealed class OptionsLocalizerOptions<TOptions>
    {
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        public HashSet<CultureInfo> SupportedCultures { get; } = [];

        public HashSet<string> OptionsPaths { get; } = [];
    }
}
