using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;

namespace OptionsLocalization
{
    sealed class OptionsLocalizer<TOptions> : IOptionsLocalizer, IOptionsLocalizer<TOptions>
    {
        private readonly IOptions<OptionsLocalizerOptions<TOptions>> options;
        private readonly IOptionsMonitor<TOptions> optionsMonitor;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public TOptions CurrentValue => this.Get(Thread.CurrentThread.CurrentCulture);

        public IReadOnlyCollection<CultureInfo> SupportedCultures => this.options.Value.OptionsCultures;

        public OptionsLocalizer(
            IOptions<OptionsLocalizerOptions<TOptions>> options,
            IOptionsMonitor<TOptions> optionsMonitor)
        {
            this.options = options;
            this.optionsMonitor = optionsMonitor;

            this.OnChange(WriteToValueFile);
        }

        public TOptions Get(string culture)
        {
            return this.Get(CultureInfo.GetCultureInfo(culture));
        }

        public TOptions Get(CultureInfo culture)
        {
            return this.optionsMonitor.Get(culture.Name);
        }

        /// <summary>
        /// 监听选项变化
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public IDisposable? OnChange(Action<TOptions, CultureInfo> listener)
        {
            return this.optionsMonitor.OnChange(OnChange);
            void OnChange(TOptions optionsValue, string? name)
            {
                if (OptionsLocalizer.TryGetCultureInfo(name, out var culture) == false)
                {
                    culture = this.options.Value.DefaultCulture;
                }
                listener(optionsValue, culture);
            }
        }

        public void WriteToValueFiles()
        {
            foreach (var culture in this.options.Value.OptionsCultures)
            {
                var optionsValue = this.Get(culture);
                this.WriteToValueFile(optionsValue, culture);
            }
        }

        /// <summary>
        /// 保存资源到 .value 文件
        /// </summary>
        /// <param name="optionsValue"></param>
        /// <param name="culture"></param>
        private void WriteToValueFile(TOptions optionsValue, CultureInfo culture)
        {
            var optionsPath = this.options.Value.OptionsPath;
            if (Directory.Exists(optionsPath))
            {
                try
                {
                    var valueFilePath = Path.Combine(optionsPath, $"{culture.Name}.json.value");
                    var valueJson = JsonSerializer.SerializeToUtf8Bytes(optionsValue, jsonSerializerOptions);                   

                    using var valueFileStream = File.Create(valueFilePath);
                    valueFileStream.Write("// 这是自动生成的完整语言文件内容，删除或修改此文件对应用没有影响\r\n"u8);
                    valueFileStream.Write(valueJson);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}