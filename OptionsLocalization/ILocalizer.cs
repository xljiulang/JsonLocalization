namespace OptionsLocalization
{
    /// <summary>
    /// 本地化选项工具接口
    /// </summary>
    /// <typeparam name="TOptions">本地化选项类型</typeparam>
    public interface ILocalizer<out TOptions>
    {
        /// <summary>
        /// 获取当前线程的语言区域对应的本地化选项
        /// </summary>
        TOptions Current { get; }

        /// <summary>
        /// 获取指定语言区域的本地化选项
        /// </summary>
        /// <param name="culture">指定语言区域名称</param>
        /// <returns></returns>
        TOptions Get(string culture);
    }
}
