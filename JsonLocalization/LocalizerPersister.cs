using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JsonLocalization
{
    sealed partial class LocalizerPersister<TTLocale>
    {
        private readonly IOptions<LocalizerOptions> options;
        private readonly IOptionsMonitor<TTLocale> localeFactory;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public LocalizerPersister(
            IOptions<LocalizerOptions> options,
            IOptionsMonitor<TTLocale> localeFactory)
        {
            this.options = options;
            this.localeFactory = localeFactory;
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            foreach (var jsonFile in Directory.GetFiles(this.options.Value.CulturesDirectory, "*.json"))
            {
                var localeName = Path.GetFileNameWithoutExtension(jsonFile);
                var locale = this.localeFactory.Get(localeName);
                var localeFile = new LocalizerFile<TTLocale>
                {
                    Cultures = new Dictionary<string, TTLocale> { [localeName] = locale }
                };

                var jsonContent = JsonSerializer.SerializeToUtf8Bytes(localeFile, jsonSerializerOptions);
                await File.WriteAllBytesAsync(jsonFile + ".txt", jsonContent, cancellationToken);
            }
        }
    }
}
