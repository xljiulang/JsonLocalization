using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;

namespace OptionsLocalization
{
    sealed class LocalizerBuilder : ILocalizerBuilder
    {
        public required CultureInfo DefaultCulture { get; init; }

        public required IServiceCollection Services { get; init; }

        public required IConfiguration Configuration { get; init; }

        /// <summary>
        /// 配置本地化数据到指定的选项类
        /// </summary>
        /// <typeparam name="TOptions"></typeparam> 
        /// <returns></returns>
        public ILocalizerBuilder Configure<TOptions>() where TOptions : class, new()
        {
            var optionsPath = LocalizerOptions.GetOptionsPath<TOptions>();
            foreach (var jsonFile in Directory.GetFiles(optionsPath, "*.json"))
            {
                var culture = Path.GetFileNameWithoutExtension(jsonFile);
                var key = $"{LocalizerOptions.LocalizationPath}:{typeof(TOptions).Name}:{culture}";
                var configuration = this.Configuration.GetSection(key);

                if (culture.Equals(this.DefaultCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    this.Services.Configure<TOptions>(configuration);
                }
                else
                {
                    this.Services.Configure<TOptions>(culture, configuration);
                }
            }

            this.Services.TryAddTransient<IOptionsFactory<TOptions>, LocalizerFactory<TOptions>>();
            this.Services.TryAddSingleton<ILocalizer<TOptions>, Localizer<TOptions>>();
            this.Services.TryAddTransient(s => s.GetRequiredService<ILocalizer<TOptions>>().Current);
            return this;
        }
    }
}