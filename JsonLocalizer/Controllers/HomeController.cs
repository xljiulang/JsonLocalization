using JsonLocalization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JsonLocalizer.Controllers
{
    public class HomeController
    {
        [HttpGet("/")]
        public Locale Index([FromServices] Locale locale, [FromServices] ILocalizer<Locale> localizer)
        { 
            Debug.Assert(locale == localizer.Current);
            return locale;
        }
    }
}
