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
    sealed class LocalizerBuilder : ILocalizerBuilder
    {
        public required CultureInfo DefaultCulture { get; init; }

        public required IServiceCollection Services { get; init; }

        public required IConfiguration Configuration { get; init; }


        public ILocalizerBuilder Configure<TOptions>() where TOptions : class, new()
        {
            var optionsPath = FindOptionsPath<TOptions>();
            var optionsCultures = FindOptionsCultures(optionsPath).ToArray();

            foreach (var culture in optionsCultures)
            {
                var key = $"{Localizer.LocalizationRoot}:{typeof(TOptions).Name}:{culture}";
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

            this.Services.Configure<LocalizerOptions<TOptions>>(options =>
            {
                options.Cultures = optionsCultures;
                options.OptionsPath = optionsPath;
                options.DefaultCulture = this.DefaultCulture;
            });

            this.Services.TryAddTransient<IOptionsFactory<TOptions>, LocalizerFactory<TOptions>>();
            this.Services.TryAddSingleton<ILocalizer<TOptions>, Localizer<TOptions>>();
            this.Services.TryAddTransient(s => s.GetRequiredService<ILocalizer<TOptions>>().Current);
            return this;
        }


        private static string FindOptionsPath<TOptions>()
        {
            var optionsPath = Path.Combine(Localizer.LocalizationRoot, typeof(TOptions).Name);
            if (Path.Exists(optionsPath))
            {
                return optionsPath;
            }

            foreach (var path in Directory.GetDirectories(Localizer.LocalizationRoot))
            {
                var optionsDirName = Path.GetFileName(path);
                if (typeof(TOptions).Name.Equals(optionsDirName, StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
            }

            return optionsPath;
        }


        private static IEnumerable<string> FindOptionsCultures(string optionsPath)
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