using JsonLocalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Threading;

namespace JsonLocalizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddJsonLocalization<Locale>("zh");

            var app = builder.Build();
            app.Use(next => ctx =>
            {
                Thread.CurrentThread.CurrentCulture = GetCultureInfo();
                CultureInfo GetCultureInfo()
                {
                    if (ctx.Request.Query.TryGetValue("culture", out var culture))
                    {
                        try
                        {
                            return CultureInfo.GetCultureInfo(culture!);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    return ctx.RequestServices.GetRequiredService<IOptions<JsonLocalizationOptions>>().Value.DefaultLocale;
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
