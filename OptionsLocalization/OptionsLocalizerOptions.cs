using System;
using System.Collections.Generic;
using System.Globalization;

namespace OptionsLocalization
{
    sealed class OptionsLocalizerOptions<TOptions>
    {
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        public HashSet<string> Cultures { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> OptionsPaths { get; } = [];
    }
}
