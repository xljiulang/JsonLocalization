using WebApp.Controllers;

namespace WebApp
{
    public class Locale
    {
        public string Key1 { get; set; } = "Hello";

        public string Key2 { get; set; } = "Word";

        public HomeLocale Home { get; set; } = new HomeLocale();
    }
}
