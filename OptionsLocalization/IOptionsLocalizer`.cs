using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace OptionsLocalization
{
    /// <summary>
    /// 本地化选项工具接口
    /// </summary>
    /// <typeparam name="TOptions">本地化选项类型</typeparam>
    public interface IOptionsLocalizer<out TOptions>
    {
        /// <summary>
        /// 获取当前线程的语言区域对应的本地化选项
        /// </summary>
        TOptions CurrentValue { get; }

        /// <summary>
        /// 选项的缺省语言区域
        /// </summary>
        CultureInfo DefaultCulture { get; }

        /// <summary>
        /// 获取选项已支持的语言区域
        /// </summary>
        ReadOnlyCollection<string> SupportedCultures { get; }

        /// <summary>
        /// 获取指定语言区域的本地化选项
        /// </summary>
        /// <param name="culture">指定语言区域名称</param>
        /// <returns></returns>
        TOptions Get(string culture);

        /// <summary>
        /// 监听选项变化
        /// </summary>
        /// <param name="listener">监听器</param>
        /// <returns></returns>
        IDisposable? OnChange(Action<TOptions, string> listener);
    }
}
