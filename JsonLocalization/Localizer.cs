using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;

namespace JsonLocalization
{
    /// <summary>
    /// 本地化工具
    /// </summary>
    /// <typeparam name="TOptions">本地化数据</typeparam>
    sealed class Localizer<TOptions> : ILocalizer<TOptions>
    {
        private readonly IOptions<LocalizerOptions> options;
        private readonly IOptionsMonitor<TOptions> optionsMonitor;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public TOptions Current => this.Get(Thread.CurrentThread.CurrentCulture.Name);


        public Localizer(IOptions<LocalizerOptions> options, IOptionsMonitor<TOptions> optionsMonitor)
        {
            this.options = options;
            this.optionsMonitor = optionsMonitor;

            var optionsPath = LocalizerOptions.GetOptionsPath<TOptions>();
            foreach (var jsonFile in Directory.GetFiles(optionsPath, "*.json"))
            {
                var culture = Path.GetFileNameWithoutExtension(jsonFile);
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
            if (string.IsNullOrEmpty(culture))
            {
                culture = this.options.Value.DefaultCulture.Name;
            }

            try
            {
                var valueJson = JsonSerializer.SerializeToUtf8Bytes(optionsValue, jsonSerializerOptions);
                var valueFile = Path.Combine(LocalizerOptions.GetOptionsPath<TOptions>(), $"{culture}.json.value");
                using var fileStream = File.Create(valueFile);
                fileStream.Write("// 这是自动生成的语言文件的完整键和值\r\n"u8);
                fileStream.Write(valueJson);
            }
            catch (Exception)
            {
            }
        }
    }
}
