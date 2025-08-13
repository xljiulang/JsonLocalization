using JsonLocalizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
        /// <param name="localesDirectory">本地化数据的 json 文件目录</param>
        /// <returns></returns>
        public static IHostApplicationBuilder AddJsonLocalizer<TLocale>(this IHostApplicationBuilder builder, string localesDirectory = "locales")
            where TLocale : class, new()
        {
            builder.Services.AddOptions();

            var locales = nameof(LocaleFile<TLocale>.Locales);
            builder.Services.Configure<TLocale>(builder.Configuration.GetSection($"{locales}:default"));
            foreach (var jsonFile in Directory.GetFiles(localesDirectory, "*.json"))
            {
                builder.Configuration.AddJsonFile(jsonFile, optional: true, reloadOnChange: true);
                var localeName = Path.GetFileNameWithoutExtension(jsonFile);
                builder.Services.Configure<TLocale>(localeName, builder.Configuration.GetSection($"{locales}:{localeName}"));
            }

            builder.Services.TryAddTransient<IOptionsFactory<TLocale>, LocaleFactory<TLocale>>();
            builder.Services.TryAddTransient(s => s.GetRequiredService<IOptionsMonitor<TLocale>>().Get(Thread.CurrentThread.CurrentCulture.Name));

            builder.Services.TryAddSingleton(s => new LocalePersister<TLocale>(localesDirectory, s.GetRequiredService<IOptionsMonitor<TLocale>>()));
            builder.Services.AddHostedService<LocalePersisterService<TLocale>>();
            return builder;
        }
    }
}