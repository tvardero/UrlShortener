using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using React.AspNet;
using UrlShortener.BLL;
using UrlShortener.BLL.CustomServices;
using UrlShortener.DataAccess;
using UrlShortener.MVC.Controllers;

internal class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddAppDbContext()
            .AddAppIdentity();

        builder.AddAppDependencyInjection();

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddControllersWithViews();

        builder.Services.AddMemoryCache(o => o.ExpirationScanFrequency = TimeSpan.FromMinutes(10));

        builder.Services.AddHsts(o => o.MaxAge = TimeSpan.FromDays(365));

        var app = builder.Build();

        app.EnsureDatabaseMigrated()
            .EnsureAdminProfileIsCreated();

        app.UseHttpsRedirection()
            .UseHsts();

        app.UseDeveloperExceptionPage()
            .UseStatusCodePages();
        // TODO: Для проду UseExceptionHandler + створити лендінг сторінку для помилок 4хх та 5хх

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "redirect",
            pattern: "go/{hash:regex([0-9A-Za-z_=]{{" + HashGeneratorService.HashLength + "}})}",
            defaults: new
            {
                controller = nameof(RedirectController).Replace("Controller", string.Empty),
                action = nameof(RedirectController.RedirectToDestination)
            });

        app.MapControllerRoute(
            name: "action",
            pattern: "{action}",
            defaults: new
            {
                controller = nameof(HomeController).Replace("Controller", string.Empty)
            });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action}",
            defaults: new
            {
                controller = nameof(HomeController).Replace("Controller", string.Empty),
                action = nameof(HomeController.Index)
            });

        ServiceProvider = app.Services;

        app.Run();
    }
}