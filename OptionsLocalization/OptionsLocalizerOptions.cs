using System.Collections.Generic;
using System.Globalization;

namespace OptionsLocalization
{
    sealed class OptionsLocalizerOptions<TOptions>
    {
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// 支持的语言区域-语言区域文件所在的目录
        /// </summary>
        public Dictionary<CultureInfo, HashSet<string>> SupportedCultures { get; } = [];
    }
}
