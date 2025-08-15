using System;
using System.IO;

namespace OptionsLocalization
{
    /// <summary>
    /// 本地化选项工具
    /// </summary>
    public class Localizer
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
    }
}
