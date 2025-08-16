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
        private readonly IServiceCollection services;
        private readonly IConfiguration configuration;

        public OptionsLocalizerBuilder(CultureInfo defaultCulture, IServiceCollection services, IConfiguration configuration)
        {
            this.defaultCulture = defaultCulture;
            this.services = services;
            this.configuration = configuration;
        }

        public IOptionsLocalizerBuilder Configure<TOptions>() where TOptions : class, new()
        {
            var optionsPath = FindOptionsPath<TOptions>();
            var optionsCultures = FindOptionsCultures(optionsPath).ToArray().AsReadOnly();

            foreach (var culture in optionsCultures)
            {
                var key = $"{OptionsLocalizer.LocalizationRoot}:{typeof(TOptions).Name}:{culture}";
                var configuration = this.configuration.GetSection(key);

                if (culture.Equals(this.defaultCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    this.services.Configure<TOptions>(configuration);
                }
                else
                {
                    this.services.Configure<TOptions>(culture, configuration);
                }
            }

            this.services.Configure<OptionsLocalizerOptions<TOptions>>(options =>
            {
                options.Cultures = optionsCultures;
                options.OptionsPath = optionsPath;
                options.DefaultCulture = this.defaultCulture;
            });

            this.services.TryAddTransient<IOptionsFactory<TOptions>, CultureOptionsFactory<TOptions>>();
            this.services.TryAddSingleton<OptionsLocalizer<TOptions>>();
            this.services.TryAddSingleton<IOptionsLocalizer<TOptions>>(s => s.GetRequiredService<OptionsLocalizer<TOptions>>());
            this.services.TryAddTransient(s => s.GetRequiredService<OptionsLocalizer<TOptions>>().CurrentValue);
            this.services.AddSingleton<IOptionsLocalizer>(s => s.GetRequiredService<OptionsLocalizer<TOptions>>());
            this.services.AddHostedService<OptionsLocalizerHostedService>();
            return this;
        }


        private static string? FindOptionsPath<TOptions>()
        {
            var optionsPaths = Directory.GetDirectories(OptionsLocalizer.LocalizationRoot);
            foreach (var optionsPath in optionsPaths)
            {
                if (typeof(TOptions).Name.Equals(Path.GetFileName(optionsPath)))
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


        private static IEnumerable<string> FindOptionsCultures(string? optionsPath)
        {
            if (Directory.Exists(optionsPath))
            {
                foreach (var jsonFile in Directory.GetFiles(optionsPath, "*.json"))
                {
                    yield return Path.GetFileNameWithoutExtension(jsonFile);
                }
            }
        }
    }
}