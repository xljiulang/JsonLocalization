using JsonLocalization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApp.Controllers
{
    public class HomeController
    {
        /// <summary>
        /// 本地化工具示例
        /// </summary>
        /// <param name="locale">当前线程对应的本地化数据</param>
        /// <param name="localizer">本地化工具</param>
        /// <returns></returns>
        [HttpGet("/")]
        public Locale Index(
            [FromServices] Locale locale,
            [FromServices] ILocalizer<Locale> localizer)
        {
            Debug.Assert(locale == localizer.Current);
            return locale;
        }
    }
}
