using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace OptionsLocalization
{
    /// <summary>
    /// 本地化选项构建器接口
    /// </summary>
    public interface IOptionsLocalizerBuilder
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
        /// 绑定配置到到指定的选项类型
        /// </summary>
        /// <typeparam name="TOptions">选项类型</typeparam>
        /// <returns></returns>
        IOptionsLocalizerBuilder Configure<TOptions>() where TOptions : class, new();
    }
}
