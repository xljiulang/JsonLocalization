using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Threading;

namespace JsonLocalizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddJsonLocalizer<Locale>();

            var app = builder.Build();
            app.Use(next => ctx =>
            {
                if (ctx.Request.Query.TryGetValue("culture", out var culture))
                {
                    var cultureName = culture.ToString();
                    if (cultureName != null)
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
                    }
                }
                return next(ctx);
            });

            app.Map("/", (Locale locale) =>
            {
                return locale;
            });
            app.Run();
        }
    }

    public class Locale
    {
        public string Key1 { get; set; } = "Hello";

        public string Key2 { get; set; } = "Word";

        public int Key3 { get; set; } = 5;
    }
}
