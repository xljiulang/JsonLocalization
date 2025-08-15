using System.Collections.Generic;
using System.Globalization;

namespace OptionsLocalization
{
    sealed class LocalizerOptions<TOptions>
    {
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        public string OptionsPath { get; set; } = string.Empty;

        public string[] Cultures { get; set; } = [];
    }
}
