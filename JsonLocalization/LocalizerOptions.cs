using System;
using System.Globalization;

namespace JsonLocalization
{
    /// <summary>
    /// 本地化选项
    /// </summary>
    public class LocalizerOptions
    {
        /// <summary>
        /// 获取默认语言区域
        /// </summary>
        public CultureInfo DefaultCulture { get; internal set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// 获取本地化数据的 json 文件目录
        /// 默认值为 locales
        /// </summary>
        public string CulturesDirectory { get; internal set; } = "cultures";

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
    }
}
