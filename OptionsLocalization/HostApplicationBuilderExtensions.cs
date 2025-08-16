using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OptionsLocalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// IHostApplicationBuilder扩展
    /// </summary>
    public static class HostApplicationBuilderExtensions
    {
        private record LocalizationRoot(string Value);

        /// <summary>
        /// 添加​本地化选项工具​
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultCulture">选项的缺省语言区域</param>
        /// <param name="localizationRoot">本地化选项的资源文件根目录</param> 
        /// <returns></returns>
        public static IOptionsLocalizerBuilder AddOptionsLocalizer(
            this IHostApplicationBuilder builder,
            string defaultCulture,
            string localizationRoot = "localizations")
        {
            return builder.AddOptionsLocalizer(CultureInfo.GetCultureInfo(defaultCulture), localizationRoot);
        }

        /// <summary>
        /// 添加​本地化选项工具​
        /// </summary> 
        /// <param name="builder"></param>
        /// <param name="defaultCulture">选项的缺省语言区域</param> 
        /// <param name="localizationRoot">本地化选项的资源文件根目录</param> 
        /// <returns></returns>
        public static IOptionsLocalizerBuilder AddOptionsLocalizer(
            this IHostApplicationBuilder builder,
            CultureInfo defaultCulture,
            string localizationRoot = "localizations")
        {
            CheckLocalizationRoot(localizationRoot, builder.Services);

            foreach (var optionsPath in Directory.GetDirectories(localizationRoot))
            {
                foreach (var jsonFilePath in Directory.GetFiles(optionsPath, "*.json"))
                {
                    if (ContainsJsonFile(builder.Configuration.Sources, jsonFilePath) == false)
                    {
                        builder.Configuration.Add<CultureJsonLocalizerConfigurationSource>(s =>
                        {
                            s.Path = jsonFilePath;
                            s.Optional = true;
                            s.ReloadOnChange = true;
                            s.ResolveFileProvider();
                        });
                    }
                }
            }

            return new OptionsLocalizerBuilder(defaultCulture, localizationRoot, builder.Services, builder.Configuration);
        }

        private static void CheckLocalizationRoot(string localizationRoot, IServiceCollection services)
        {
            ArgumentException.ThrowIfNullOrEmpty(localizationRoot);

            if (localizationRoot.StartsWith('.') ||
                Path.IsPathRooted(localizationRoot) ||
                Path.GetDirectoryName(localizationRoot.AsSpan()).Length > 0)
            {
                throw new ArgumentException("Localization root must be a directory name.", nameof(localizationRoot));
            }

            var descriptor = services.FirstOrDefault(i => i.ServiceType == typeof(LocalizationRoot));
            if (descriptor == null)
            {
                services.AddSingleton(new LocalizationRoot(localizationRoot));
            }
            else if (descriptor.ImplementationInstance is LocalizationRoot root && root.Value != localizationRoot)
            {
                throw new InvalidOperationException($"Localization root has already been set to '{root.Value}'.");
            }

            Directory.CreateDirectory(localizationRoot);
        }

        private static bool ContainsJsonFile(IList<IConfigurationSource> sources, string jsonFilePath)
        {
            var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return sources.OfType<CultureJsonLocalizerConfigurationSource>().Any(i => jsonFilePath.Equals(i.Path, comparison));
        }
    }
}