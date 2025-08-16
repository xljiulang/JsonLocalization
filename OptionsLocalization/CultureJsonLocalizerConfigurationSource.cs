using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;
using System.Linq;

namespace OptionsLocalization
{
    /// <summary>
    /// 本地化配置源
    /// </summary>
    sealed class CultureJsonLocalizerConfigurationSource : JsonConfigurationSource
    {
        public string? LocalizationRoot { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (this.LocalizationRoot == null)
            {
                throw new InvalidOperationException("LocalizationRoot must be set.");
            }

            EnsureDefaults(builder);
            return new LocalizerConfigurationProvider(this, this.LocalizationRoot);
        }

        private class LocalizerConfigurationProvider : JsonConfigurationProvider
        {
            private readonly string localizationRoot;

            public LocalizerConfigurationProvider(CultureJsonLocalizerConfigurationSource source, string localizationRoot)
                : base(source)
            {
                this.localizationRoot = localizationRoot;
            }

            public override void Load(Stream stream)
            {
                base.Load(stream);

                var filePath = this.Source.Path;
                if (filePath == null)
                {
                    return;
                }

                var culture = System.IO.Path.GetFileNameWithoutExtension(filePath);
                var optionsPath = System.IO.Path.GetDirectoryName(filePath);
                var optionsDirName = System.IO.Path.GetFileName(optionsPath);

                var keyPrefix = $"{this.localizationRoot}:{optionsDirName}:{culture}";
                this.Data = this.Data.ToDictionary(kv => $"{keyPrefix}:{kv.Key}", kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
