using System;
using System.Globalization;

namespace JsonLocalization
{
    public class JsonLocalizationOptions
    {
        /// <summary>
        /// 获取默认语言区域
        /// </summary>
        public CultureInfo DefaultLocale { get; internal set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// 获取本地化数据的 json 文件目录
        /// 默认值为 locales
        /// </summary>
        public string LocalesDirectory { get; internal set; } = "locales";

        /// <summary>
        /// 获取指定语言区域，不存在则返回默认语言区域
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public CultureInfo GetLocale(string? culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return this.DefaultLocale;
            }

            try
            {
                return CultureInfo.GetCultureInfo(culture);
            }
            catch (Exception)
            {
                return this.DefaultLocale;
            }
        }
    }
}
