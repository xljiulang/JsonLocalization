using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace OptionsLocalization
{
    sealed class OptionsLocalizerOptions<TOptions>
    {
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        public string? OptionsPath { get; set; } 

        public ReadOnlyCollection<string> Cultures { get; set; } = Array.Empty<string>().AsReadOnly();
    }
}
