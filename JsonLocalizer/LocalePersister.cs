using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JsonLocalization
{
    sealed partial class LocalePersister<TTLocale>
    {
        private readonly string localesDirectory;
        private readonly IOptionsMonitor<TTLocale> localeOptions;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public LocalePersister(string localesDirectory, IOptionsMonitor<TTLocale> localeOptions)
        {
            this.localesDirectory = localesDirectory;
            this.localeOptions = localeOptions;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            foreach (var jsonFile in Directory.GetFiles(this.localesDirectory, "*.json"))
            {
                var localeName = Path.GetFileNameWithoutExtension(jsonFile);
                var locale = this.localeOptions.Get(localeName);
                var localeFile = new LocaleFile<TTLocale>
                {
                    Locales = new Dictionary<string, TTLocale> { [localeName] = locale }
                };

                var jsonContent = JsonSerializer.SerializeToUtf8Bytes(localeFile, jsonSerializerOptions);
                await File.WriteAllBytesAsync(jsonFile, jsonContent, cancellationToken);
            }
        }
    }
}
