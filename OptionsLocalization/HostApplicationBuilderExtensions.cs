using Microsoft.Extensions.Configuration;
using OptionsLocalization;
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
        /// 添加​本地化选项工具​
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultCulture">选项的缺省语言区域</param> 
        /// <returns></returns>
        public static IOptionsLocalizerBuilder AddOptionsLocalizer(this IHostApplicationBuilder builder, string defaultCulture)
        {
            return builder.AddOptionsLocalizer(CultureInfo.GetCultureInfo(defaultCulture));
        }

        /// <summary>
        /// 添加​本地化选项工具​
        /// </summary> 
        /// <param name="builder"></param>
        /// <param name="defaultCulture">选项的缺省语言区域</param> 
        /// <returns></returns>
        public static IOptionsLocalizerBuilder AddOptionsLocalizer(this IHostApplicationBuilder builder, CultureInfo defaultCulture)
        {
            foreach (var optionsPath in Directory.GetDirectories(OptionsLocalizer.LocalizationRoot))
            {
                foreach (var jsonFile in Directory.GetFiles(optionsPath, "*.json"))
                {
                    if (builder.Configuration.Sources.OfType<CultureJsonLocalizerConfigurationSource>().Any(i => i.Path == jsonFile) == false)
                    {
                        builder.Configuration.Add<CultureJsonLocalizerConfigurationSource>(s =>
                        {
                            s.Path = jsonFile;
                            s.Optional = true;
                            s.ReloadOnChange = true;
                            s.ResolveFileProvider();
                        });
                    }
                }
            }

            return new OptionsLocalizerBuilder(defaultCulture, builder.Services, builder.Configuration);
        }
    }
}