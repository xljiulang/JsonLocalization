using JsonLocalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;

namespace JsonLocalizer
{
    sealed class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.AddLocalizer<Locale>("zh");
            builder.Services.AddControllers();

            var app = builder.Build();
            app.Use(next => context =>
            {
                context.Request.Query.TryGetValue("culture", out var culture);
                var options = context.RequestServices.GetRequiredService<IOptions<LocalizerOptions>>();
                Thread.CurrentThread.CurrentCulture = options.Value.GetCulture(culture);
                return next(context);
            });

            app.MapControllers();
            app.Run();
        }
    }
}
