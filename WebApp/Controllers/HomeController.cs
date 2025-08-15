using Microsoft.AspNetCore.Mvc;
using OptionsLocalization;
using System.Diagnostics;

namespace WebApp.Controllers
{
    public class HomeController
    {
        /// <summary>
        /// 本地化工具示例
        /// </summary>
        /// <param name="appOptions"></param>
        /// <param name="homeOptions"></param>
        /// <param name="localizer"></param>
        /// <returns></returns>
        [HttpGet("/")]
        public HomeOptions Index(
            [FromServices] AppOptions appOptions,
            [FromServices] HomeOptions homeOptions,
            [FromServices] ILocalizer<HomeOptions> localizer)
        {
            Debug.Assert(homeOptions == localizer.Current);
            return homeOptions;
        }
    }
}
