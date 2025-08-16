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
            var optionsPath = FindOptionsPath<TOptions>(this.localizationRoot);
            var optionsCultures = FindOptionsCultures(optionsPath).Distinct().ToArray().AsReadOnly();

            this.services.Configure<OptionsLocalizerOptions<TOptions>>(options =>
            {
                options.OptionsPath = optionsPath;
                options.OptionsCultures = optionsCultures;
                options.DefaultCulture = this.defaultCulture;
            });

            foreach (var culture in optionsCultures)
            {
                var key = $"{nameof(OptionsLocalization)}:{typeof(TOptions).Name}:{culture}";
                var configuration = this.configuration.GetSection(key);
                var optionsName = this.defaultCulture.Equals(culture) ? Options.DefaultName : culture.Name;
                this.services.Configure<TOptions>(optionsName, configuration);
            }

            this.services.TryAddTransient<IOptionsFactory<TOptions>, CultureOptionsFactory<TOptions>>();
            this.services.TryAddSingleton<OptionsLocalizer<TOptions>>();
            this.services.TryAddSingleton<IOptionsLocalizer<TOptions>>(s => s.GetRequiredService<OptionsLocalizer<TOptions>>());
            this.services.TryAddTransient(s => s.GetRequiredService<OptionsLocalizer<TOptions>>().CurrentValue);
            this.services.AddSingleton<IOptionsLocalizer>(s => s.GetRequiredService<OptionsLocalizer<TOptions>>());
            this.services.AddHostedService<OptionsLocalizerHostedService>();

            return this;
        }


        private static string? FindOptionsPath<TOptions>(string localizationRoot)
        {
            var optionsPaths = Directory.GetDirectories(localizationRoot);
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


        private static IEnumerable<CultureInfo> FindOptionsCultures(string? optionsPath)
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