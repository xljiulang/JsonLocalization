using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace OptionsLocalization
{
    /// <summary>
    /// 本地化选项工具
    /// </summary>
    public static class OptionsLocalizer
    {
        private static string root = "localizations";

        /// <summary>
        /// 获取或设置本地化选项的资源文件根目录
        /// 默认为 localizations
        /// </summary>
        public static string LocalizationRoot
        {
            get
            {
                Directory.CreateDirectory(root);
                return root;
            }
            set
            {
                if (Path.GetDirectoryName(value.AsSpan()).Length > 0)
                {
                    throw new ArgumentException(value);
                }
                root = value;
            }
        }

        /// <summary>
        /// 设置当前线程的语言区域为指定值
        /// </summary>
        /// <param name="culture">语言区域</param>
        /// <param name="fallback">回退的语言区域</param>
        public static void SetCurrentThreadCulture(string? culture, string fallback)
        {
            SetCurrentThreadCulture(culture, CultureInfo.GetCultureInfo(fallback));
        }

        /// <summary>
        /// 设置当前线程的语言区域为指定值
        /// </summary>
        /// <param name="culture">语言区域</param>
        /// <param name="fallback">回退的语言区域</param>
        public static void SetCurrentThreadCulture(string? culture, CultureInfo fallback)
        {
            if (string.IsNullOrEmpty(culture))
            {
                SetCurrentThreadCulture(fallback);
                return;
            }

            try
            {
                SetCurrentThreadCulture(CultureInfo.GetCultureInfo(culture));
            }
            catch (Exception)
            {
                SetCurrentThreadCulture(fallback);
            }
        }

        private static void SetCurrentThreadCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
