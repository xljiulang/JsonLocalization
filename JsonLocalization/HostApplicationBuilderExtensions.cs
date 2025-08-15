using JsonLocalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        /// <summary>
        /// 添加​本地化工具​      
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultCulture">默认语言区域</param> 
        /// <returns></returns>
        public static ILocalizerBuilder AddLocalizer(this IHostApplicationBuilder builder, string defaultCulture)
        {
            return builder.AddLocalizer(CultureInfo.GetCultureInfo(defaultCulture));
        }

        /// <summary>
        /// 添加​本地化工具​      
        /// </summary> 
        /// <param name="builder"></param>
        /// <param name="defaultCulture">默认语言区域</param> 
        /// <returns></returns>
        public static ILocalizerBuilder AddLocalizer(this IHostApplicationBuilder builder, CultureInfo defaultCulture)
        {
            builder.Configuration.AddLocalizer();
            return builder.Services.AddLocalizer(builder.Configuration, defaultCulture);
        }


        private static void AddLocalizer(this IConfigurationBuilder builder)
        {
            Directory.CreateDirectory(LocalizerOptions.LocalizationPath);
            foreach (var optionsPath in Directory.GetDirectories(LocalizerOptions.LocalizationPath))
            {
                var jsonFiles = Directory.GetFiles(optionsPath, "*.json");
                foreach (var jsonFile in jsonFiles)
                {
                    if (builder.Sources.OfType<LocalizerConfigurationSource>().Any(i => i.Path == jsonFile) == false)
                    {
                        builder.Add<LocalizerConfigurationSource>(s =>
                        {
                            s.Path = jsonFile;
                            s.Optional = true;
                            s.ReloadOnChange = true;
                            s.ResolveFileProvider();
                        });
                    }
                }
            }
        }

        private static LocalizerBuilder AddLocalizer(this IServiceCollection services, IConfiguration configuration, CultureInfo defaultCulture)
        {
            // 配置参数到 LocalizerOptions
            services.PostConfigure<LocalizerOptions>(options =>
            {
                options.DefaultCulture = defaultCulture;
            });

            return new LocalizerBuilder
            {
                Services = services,
                Configuration = configuration,
                DefaultCulture = defaultCulture,
            };
        }
    }
}