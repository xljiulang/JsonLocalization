using Microsoft.AspNetCore.Mvc;

namespace JsonLocalizer.Controllers
{
    public class HomeController
    {
        [HttpGet("/")]
        public Locale Index([FromServices] Locale locale)
        {
            return locale;
        }
    }
}
