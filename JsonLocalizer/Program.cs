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
    sealed class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddJsonLocalization<Locale>("zh");
            builder.Services.AddControllers();

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

            app.MapControllers();
            app.Run();
        }
    }
}
