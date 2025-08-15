using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace JsonLocalization
{
    /// <summary>
    /// 本地化构建器接口
    /// </summary>
    public interface ILocalizerBuilder
    {
        /// <summary>
        /// 获取默认的语言区域
        /// </summary>
        CultureInfo DefaultCulture { get; }

        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// 获取配置
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// 配置本地化数据到指定的选项类
        /// </summary>
        /// <typeparam name="TOptions">选项类型</typeparam>
        /// <returns></returns>
        ILocalizerBuilder Configure<TOptions>() where TOptions : class, new();
    }
}
