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
            return new LocalizerConfigurationProvider(this);
        }

        private class LocalizerConfigurationProvider(LocalizerConfigurationSource source) : JsonConfigurationProvider(source)
        {
            public override void Load(Stream stream)
            {
                base.Load(stream);

                // 将语言区域添加到每个键的前缀
                var culture = System.IO.Path.GetFileNameWithoutExtension(this.Source.Path);
                this.Data = this.Data.ToDictionary(kv => $"cultures:{culture}:{kv.Key}", kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
