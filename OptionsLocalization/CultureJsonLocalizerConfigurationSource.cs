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
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new LocalizerConfigurationProvider(this);
        }

        private class LocalizerConfigurationProvider(CultureJsonLocalizerConfigurationSource source) : JsonConfigurationProvider(source)
        {
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

                var keyPrefix = $"{OptionsLocalizer.LocalizationRoot}:{optionsDirName}:{culture}";
                this.Data = this.Data.ToDictionary(kv => $"{keyPrefix}:{kv.Key}", kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
