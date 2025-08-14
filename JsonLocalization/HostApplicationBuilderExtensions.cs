using JsonLocalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// IHostApplicationBuilder扩展
    /// </summary>
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// 添加​本地化工具​      
        /// </summary>
        /// <typeparam name="TOptions">本地化的数据类型</typeparam>
        /// <param name="builder"></param>
        /// <param name="defaultCulture">默认语言区域</param>
        /// <param name="resourcesPath">本地化数据的资源文件目录</param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddLocalizer<TOptions>(this IHostApplicationBuilder builder, string defaultCulture, string resourcesPath = "cultures")
            where TOptions : class, new()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(resourcesPath);
            return builder.AddLocalizer<TOptions>(CultureInfo.GetCultureInfo(defaultCulture), resourcesPath);
        }

        /// <summary>
        /// 添加​本地化工具​      
        /// </summary>
        /// <typeparam name="TOptions">本地化的数据类型</typeparam>
        /// <param name="builder"></param>
        /// <param name="defaultCulture">默认语言区域</param>
        /// <param name="resourcesPath">本地化数据的资源文件目录</param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddLocalizer<TOptions>(this IHostApplicationBuilder builder, CultureInfo defaultCulture, string resourcesPath = "cultures")
            where TOptions : class, new()
        {
            // 配置参数到 LocalizerOptions
            builder.Services.PostConfigure<LocalizerOptions>(options =>
            {
                options.DefaultCulture = defaultCulture;
                options.ResourcesPath = resourcesPath;
            });


            foreach (var jsonFile in Directory.GetFiles(resourcesPath, "*.json"))
            {
                builder.Configuration.Add<LocalizerConfigurationSource>(s =>
                {
                    s.Path = jsonFile;
                    s.Optional = true;
                    s.ReloadOnChange = true;
                    s.ResolveFileProvider();
                });

                var culture = Path.GetFileNameWithoutExtension(jsonFile);
                var configuration = builder.Configuration.GetSection($"cultures:{culture}");

                if (culture.Equals(defaultCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    builder.Services.Configure<TOptions>(configuration);
                }
                else
                {
                    builder.Services.Configure<TOptions>(culture, configuration);
                }
            }

            builder.Services.TryAddTransient<IOptionsFactory<TOptions>, LocalizerFactory<TOptions>>();
            builder.Services.TryAddSingleton<ILocalizer<TOptions>, Localizer<TOptions>>();
            builder.Services.TryAddTransient(s => s.GetRequiredService<ILocalizer<TOptions>>().Current);
            return builder;
        }
    }
}