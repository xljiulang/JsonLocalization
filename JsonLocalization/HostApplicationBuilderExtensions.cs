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
        /// <param name="culturesDirectory">本地化数据的 json 文件相对目录</param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddLocalizer<TOptions>(this IHostApplicationBuilder builder, string defaultCulture, string culturesDirectory = "cultures")
            where TOptions : class, new()
        {
            return builder.AddLocalizer<TOptions>(CultureInfo.GetCultureInfo(defaultCulture), culturesDirectory);
        }

        /// <summary>
        /// 添加​本地化工具​      
        /// </summary>
        /// <typeparam name="TOptions">本地化的数据类型</typeparam>
        /// <param name="builder"></param>
        /// <param name="defaultCulture">默认语言区域</param>
        /// <param name="culturesDirectory">本地化数据的 json 文件相对目录</param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddLocalizer<TOptions>(this IHostApplicationBuilder builder, CultureInfo defaultCulture, string culturesDirectory = "cultures")
            where TOptions : class, new()
        {
            builder.Services.PostConfigure<LocalizerOptions>(options =>
            {
                options.DefaultCulture = defaultCulture;
                options.CulturesDirectory = culturesDirectory;
            });


            foreach (var jsonFile in Directory.GetFiles(culturesDirectory, "*.json"))
            {
                builder.Configuration.AddJsonFile(jsonFile, optional: true, reloadOnChange: true);

                var localeName = Path.GetFileNameWithoutExtension(jsonFile);
                var configuration = builder.Configuration.GetSection($"{nameof(LocalizerFile<TOptions>.Cultures)}:{localeName}");
                if (localeName.Equals(defaultCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    builder.Services.Configure<TOptions>(configuration);
                }
                else
                {
                    builder.Services.Configure<TOptions>(localeName, configuration);
                }
            }

            builder.Services.TryAddTransient<IOptionsFactory<TOptions>, LocalizerOptionsFactory<TOptions>>();
            builder.Services.TryAddSingleton<ILocalizer<TOptions>, Localizer<TOptions>>();
            builder.Services.TryAddTransient(s => s.GetRequiredService<ILocalizer<TOptions>>().Current);

            builder.Services.TryAddSingleton<LocalizerPersister<TOptions>>();
            builder.Services.AddHostedService<LocalizerPersisterService<TOptions>>();
            return builder;
        }
    }
}