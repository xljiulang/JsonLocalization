using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

            foreach (var jsonFile in Directory.GetFiles(options.Value.ResourcesPath, "*.json"))
            {
                var culture = Path.GetFileNameWithoutExtension(jsonFile);
                this.WriteToValueFile(optionsMonitor.Get(culture), culture);
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

            var resource = new LocalizerResource<TOptions>
            {
                Cultures = new Dictionary<string, TOptions> { [culture] = optionsValue }
            };

            try
            {
                var resourceJson = JsonSerializer.SerializeToUtf8Bytes(resource, jsonSerializerOptions);
                File.WriteAllBytes(Path.Combine(this.options.Value.ResourcesPath, $"{culture}.json.value"), resourceJson);
            }
            catch (Exception)
            {
            }
        }
    }
}
