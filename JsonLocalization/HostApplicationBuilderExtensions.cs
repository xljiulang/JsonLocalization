using JsonLocalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.Extensions.Hosting
{
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// 添加​本地化工具​      
        /// </summary>
        /// <typeparam name="TLocale">本地化的数据类型</typeparam>
        /// <param name="builder"></param>
        /// <param name="defaultLocale">默认语言区域</param>
        /// <param name="localesDirectory">本地化数据的 json 文件目录</param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddJsonLocalization<TLocale>(this IHostApplicationBuilder builder, string defaultLocale, string localesDirectory = "locales")
            where TLocale : class, new()
        {
            return builder.AddJsonLocalization<TLocale>(CultureInfo.GetCultureInfo(defaultLocale), localesDirectory);
        }

        /// <summary>
        /// 添加​本地化工具​      
        /// </summary>
        /// <typeparam name="TLocale">本地化的数据类型</typeparam>
        /// <param name="builder"></param>
        /// <param name="defaultLocale">默认语言区域</param>
        /// <param name="localesDirectory">本地化数据的 json 文件目录</param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddJsonLocalization<TLocale>(this IHostApplicationBuilder builder, CultureInfo defaultLocale, string localesDirectory = "locales")
            where TLocale : class, new()
        {
            builder.Services.PostConfigure<JsonLocalizationOptions>(options =>
            {
                options.DefaultLocale = defaultLocale;
                options.LocalesDirectory = localesDirectory;
            });


            foreach (var jsonFile in Directory.GetFiles(localesDirectory, "*.json"))
            {
                builder.Configuration.AddJsonFile(jsonFile, optional: true, reloadOnChange: true);

                var localeName = Path.GetFileNameWithoutExtension(jsonFile);
                var configuration = builder.Configuration.GetSection($"{nameof(LocaleFile<TLocale>.Locales)}:{localeName}");
                if (localeName.Equals(defaultLocale.Name, StringComparison.OrdinalIgnoreCase))
                {
                    builder.Services.Configure<TLocale>(configuration);
                }
                else
                {
                    builder.Services.Configure<TLocale>(localeName, configuration);
                }
            }

            builder.Services.TryAddTransient<IOptionsFactory<TLocale>, LocaleOptionsFactory<TLocale>>();
            builder.Services.TryAddTransient(s => s.GetRequiredService<IOptionsMonitor<TLocale>>().Get(Thread.CurrentThread.CurrentCulture.Name));

            builder.Services.TryAddSingleton<LocalePersister<TLocale>>();
            builder.Services.AddHostedService<LocalePersisterService<TLocale>>();
            return builder;
        }
    }
}