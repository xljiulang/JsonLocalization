using System.Collections.Generic;

namespace JsonLocalization
{
    /// <summary>
    /// 本地化工具的资源类
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    sealed class LocalizerResource<TOptions>
    {
        public required Dictionary<string, TOptions> Cultures { get; init; }
    }
}
