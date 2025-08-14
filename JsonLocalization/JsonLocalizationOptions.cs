using System.Globalization;

namespace JsonLocalization
{
    public class JsonLocalizationOptions
    {
        /// <summary>
        /// 获取默认语言区域
        /// </summary>
        public CultureInfo DefaultLocale { get; internal set; } = CultureInfo.CurrentCulture;
    }
}
