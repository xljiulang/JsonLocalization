using System.Collections.Generic;
using WebApp.Controllers;

namespace WebApp
{
    public class Locale
    {
        public string Key1 { get; set; } = "Hello";

        public string Key2 { get; set; } = "World";

        public HomeLocale Home { get; set; } = new HomeLocale();

        public Dictionary<string, string> DynamicData { get; set; } = new Dictionary<string, string>
        {
            { "Data1", "hello" },
            { "Data2", "world" }        
        };
    }
}
