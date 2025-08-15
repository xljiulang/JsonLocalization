using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;

namespace OptionsLocalization
{
    sealed class OptionsLocalizer<TOptions> : IOptionsLocalizer<TOptions>
    {
        private readonly IOptions<OptionsLocalizerOptions<TOptions>> options;
        private readonly IOptionsMonitor<TOptions> optionsMonitor;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public TOptions CurrentValue => this.Get(Thread.CurrentThread.CurrentCulture.Name);

        public CultureInfo DefaultCulture => this.options.Value.DefaultCulture;

        public ReadOnlyCollection<string> SupportedCultures => this.options.Value.Cultures;

        public OptionsLocalizer(
            IOptions<OptionsLocalizerOptions<TOptions>> options,
            IOptionsMonitor<TOptions> optionsMonitor)
        {
            this.options = options;
            this.optionsMonitor = optionsMonitor;

            foreach (var culture in options.Value.Cultures)
            {
                var optionsValue = optionsMonitor.Get(culture);
                this.WriteToValueFile(optionsValue, culture);
            }

            optionsMonitor.OnChange(this.WriteToValueFile);
        }

        public TOptions Get(string culture)
        {
            return this.optionsMonitor.Get(culture);
        }

        /// <summary>
        /// 保存资源到 .value 文件
        /// </summary>
        /// <param name="optionsValue"></param>
        /// <param name="culture"></param>
        private void WriteToValueFile(TOptions optionsValue, string? culture)
        {
            var optionsPath = options.Value.OptionsPath;
            if (Directory.Exists(optionsPath) == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(culture))
            {
                culture = this.options.Value.DefaultCulture.Name;
            }

            try
            {
                var valueFile = Path.Combine(optionsPath, $"{culture}.json.value");
                using var fileStream = File.Create(valueFile);
                var valueJson = JsonSerializer.SerializeToUtf8Bytes(optionsValue, jsonSerializerOptions);
                fileStream.Write("// 这是自动生成的语言文件的完整键和值\r\n"u8);
                fileStream.Write(valueJson);
            }
            catch (Exception)
            {
            }
        }
    }
}
