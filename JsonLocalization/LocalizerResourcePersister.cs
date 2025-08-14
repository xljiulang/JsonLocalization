using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JsonLocalization
{
    sealed class LocalizerResourcePersister<TOptions>
    {
        private readonly IOptions<LocalizerOptions> options;
        private readonly ILocalizer<TOptions> localizer;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public LocalizerResourcePersister(
            IOptions<LocalizerOptions> options,
            ILocalizer<TOptions> localizer)
        {
            this.options = options;
            this.localizer = localizer;
        }

        public async Task WriteToFilesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var jsonFile in Directory.GetFiles(this.options.Value.ResourcesPath, "*.json"))
            {
                var culture = Path.GetFileNameWithoutExtension(jsonFile);
                var options = this.localizer.Get(culture);
                var localizerFile = new LocalizerResource<TOptions>
                {
                    Cultures = new Dictionary<string, TOptions> { [culture] = options }
                };

                var jsonContent = JsonSerializer.SerializeToUtf8Bytes(localizerFile, jsonSerializerOptions);
                await File.WriteAllBytesAsync(jsonFile + ".value", jsonContent, cancellationToken);
            }
        }
    }
}
