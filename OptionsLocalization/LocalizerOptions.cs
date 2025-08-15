using System;
using System.Globalization;
using System.IO;

namespace OptionsLocalization
{
    /// <summary>
    /// 本地化选项
    /// </summary>
    public class LocalizerOptions
    {
        /// <summary>
        /// 本地化数据的资源文件目录
        /// </summary>
        public const string LocalizationPath = "localizations";

        /// <summary>
        /// 获取默认语言区域
        /// </summary>
        public CultureInfo DefaultCulture { get; internal set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// 获取指定语言区域，不存在则返回默认语言区域
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public CultureInfo GetCulture(string? culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return this.DefaultCulture;
            }

            try
            {
                return CultureInfo.GetCultureInfo(culture);
            }
            catch (Exception)
            {
                return this.DefaultCulture;
            }
        }


        internal static string GetOptionsPath<TOptions>()
        {
            var optionsPath = Path.Combine(LocalizationPath, typeof(TOptions).Name);
            if (Path.Exists(optionsPath))
            {
                return optionsPath;
            }

            foreach (var path in Directory.GetDirectories(LocalizationPath))
            {
                var optionsDirName = Path.GetFileName(path);
                if (typeof(TOptions).Name.Equals(optionsDirName, StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
            }

            return optionsPath;
        }
    }
}
