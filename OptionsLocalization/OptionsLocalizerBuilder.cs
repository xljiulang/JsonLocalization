using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OptionsLocalization
{
    sealed class OptionsLocalizerBuilder : IOptionsLocalizerBuilder
    {
        private readonly CultureInfo defaultCulture;
        private readonly string localizationRoot;
        private readonly IServiceCollection services;
        private readonly IConfiguration configuration;

        public OptionsLocalizerBuilder(
            CultureInfo defaultCulture,
            string localizationRoot,
            IServiceCollection services,
            IConfiguration configuration)
        {
            this.defaultCulture = defaultCulture;
            this.localizationRoot = localizationRoot;
            this.services = services;
            this.configuration = configuration;
        }

        public IOptionsLocalizerBuilder Configure<TOptions>() where TOptions : class, new()
        {
            this.services.TryAddTransient<IOptionsFactory<TOptions>, CultureOptionsFactory<TOptions>>();
            this.services.TryAddSingleton<OptionsLocalizer<TOptions>>();
            this.services.TryAddSingleton<IOptionsLocalizer<TOptions>>(s => s.GetRequiredService<OptionsLocalizer<TOptions>>());
            this.services.TryAddTransient(s => s.GetRequiredService<OptionsLocalizer<TOptions>>().CurrentValue);
            this.services.AddSingleton<IOptionsLocalizer>(s => s.GetRequiredService<OptionsLocalizer<TOptions>>());
            this.services.AddHostedService<OptionsLocalizerHostedService>();

            this.services.Configure<OptionsLocalizerOptions<TOptions>>(options =>
            {
                options.DefaultCulture = this.defaultCulture;
            });

            var optionsPath = FindOptionsPath<TOptions>();
            if (optionsPath == null)
            {
                return this;
            }

            var optionsCultures = FindOptionsCultures(optionsPath).ToArray();
            foreach (var culture in optionsCultures)
            {
                var key = $"{this.localizationRoot}:{typeof(TOptions).Name}:{culture}";
                var configuration = this.configuration.GetSection(key);
                var optionsName = this.defaultCulture.Equals(culture) ? Options.DefaultName : culture.Name;
                this.services.AddOptions<TOptions>(optionsName).Bind(configuration);
            }

            this.services.Configure<OptionsLocalizerOptions<TOptions>>(options =>
            {
                foreach (var culture in optionsCultures)
                {
                    if (options.SupportedCultures.TryGetValue(culture, out var optionsPaths))
                    {
                        optionsPaths.Add(optionsPath);
                    }
                    else
                    {
                        options.SupportedCultures.TryAdd(culture, [optionsPath]);
                    }
                }
            });

            return this;
        }


        private string? FindOptionsPath<TOptions>()
        {
            var optionsPaths = Directory.GetDirectories(this.localizationRoot);
            foreach (var optionsPath in optionsPaths)
            {
                if (typeof(TOptions).Name.Equals(Path.GetFileName(optionsPath), StringComparison.Ordinal))
                {
                    return optionsPath;
                }
            }

            foreach (var optionsPath in optionsPaths)
            {
                if (typeof(TOptions).Name.Equals(Path.GetFileName(optionsPath), StringComparison.OrdinalIgnoreCase))
                {
                    return optionsPath;
                }
            }
            return null;
        }


        private static IEnumerable<CultureInfo> FindOptionsCultures(string optionsPath)
        {
            if (Directory.Exists(optionsPath))
            {
                foreach (var jsonFile in Directory.GetFiles(optionsPath, "*.json"))
                {
                    var culture = Path.GetFileNameWithoutExtension(jsonFile);
                    if (OptionsLocalizer.TryGetCultureInfo(culture, out var value))
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}