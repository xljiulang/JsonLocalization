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

        private class LocalizerConfigurationProvider(LocalizerConfigurationSource source) : JsonConfigurationProvider(source)
        {
            public override void Load(Stream stream)
            {
                base.Load(stream);

                if (this.Source.Path == null)
                {
                    return;
                }

                var filePath = System.IO.Path.GetRelativePath(".", this.Source.Path);
                var keyPath = System.IO.Path.ChangeExtension(filePath, null);
                if (keyPath == null)
                {
                    return;
                }

                var keyPrefix = keyPath.Replace('\\', ':').Replace('/', ':');
                this.Data = this.Data.ToDictionary(kv => $"{keyPrefix}:{kv.Key}", kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
