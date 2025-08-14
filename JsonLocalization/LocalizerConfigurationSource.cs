using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;
using System.Linq;

namespace JsonLocalization
{
    /// <summary>
    /// 本地化配置源
    /// </summary>
    sealed class LocalizerConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new LocalizerConfigurationProvider(this);
        }

        private class LocalizerConfigurationProvider : JsonConfigurationProvider
        {
            public LocalizerConfigurationProvider(LocalizerConfigurationSource source)
                : base(source)
            {
            }

            public override void Load(Stream stream)
            {
                base.Load(stream);

                var culture = System.IO.Path.GetFileNameWithoutExtension(this.Source.Path);
                if (string.IsNullOrEmpty(culture) == false)
                {
                    this.Data = this.Data.ToDictionary(kv => $"cultures:{culture}:{kv.Key}", kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                }
            }
        }
    }
}
