using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UrlShortener.DataAccess.DbEntities;

namespace UrlShortener.DataAccess;

public static class AspNetCoreExtensions
{
    /// <summary>
    /// Додати контекст EntityFrameworkCore до DI контейнера.
    /// </summary>
    /// <param name="builder">Білдер додатка.</param>
    /// <returns>Білдер додатка для подальшого налаштування.</returns>
    public static WebApplicationBuilder AddAppDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Database");

        builder.Services.AddDbContextPool<AppDbContext>(o =>
        {
            o.UseSqlServer(connectionString);

            o.EnableDetailedErrors(builder.Environment.IsDevelopment());
            o.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

            o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        });

        return builder;
    }

    /// <summary>
    /// Додати ідентіті до DI контейнера, використовуючи контекст EntityFrameworkCore, та налаштувати параметри кукі.
    /// </summary>
    /// <param name="builder">Білдер додатка.</param>
    /// <returns>Білдер додатка для подальшого налаштування.</returns>
    public static WebApplicationBuilder AddAppIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, Role>(o =>
        {
            o.Password.RequireNonAlphanumeric = !builder.Environment.IsDevelopment();
            o.Password.RequireLowercase = !builder.Environment.IsDevelopment();

            o.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddRoles<Role>();

        builder.Services.ConfigureApplicationCookie(o =>
        {
            o.Cookie.HttpOnly = true;
            o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            o.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;

            o.ExpireTimeSpan = TimeSpan.FromDays(1);

            o.AccessDeniedPath = "/";
        });

        return builder;
    }

    public static WebApplication EnsureDatabaseMigrated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        ctx.Database.Migrate();

        return app;
    }

    public static WebApplication EnsureAdminProfileIsCreated(this WebApplication app)
    {
        var adminUsername = app.Configuration.GetValue<string>("AdminProfile:Username");
        var adminPassword = app.Configuration.GetValue<string>("AdminProfile:Password");
        var adminEmail = app.Configuration.GetValue<string>("AdminProfile:Email");
        var adminPhoneNumber = app.Configuration.GetValue<string>("AdminProfile:PhoneNumber");

        using var scope = app.Services.CreateScope();
        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        // Перевірити чи існує роль "адмін"
        var adminRole = roleManager.FindByNameAsync(Constants.AdminRoleName).Result;
        if (adminRole == null)
        {
            adminRole = new() { Name = Constants.AdminRoleName };
            var result = roleManager.CreateAsync(adminRole).Result;

            if (!result.Succeeded) throw new Exception($"Failed to create \"{adminRole.Name}\" role: {result.Errors}");
        }

        // Перевірити чи існує профіль адміна
        var admin = userManager.FindByNameAsync(adminUsername).Result;
        if (admin == null)
        {
            admin = new() { UserName = adminUsername, Email = adminEmail, PhoneNumber = adminPhoneNumber };
            var result = userManager.CreateAsync(admin, adminPassword).Result;

            if (!result.Succeeded) throw new Exception($"Failed to create admin profile: {result.Errors}");
        }

        // Якщо профіль адміна існує - перевірити пароль
        else if (!userManager.CheckPasswordAsync(admin, adminPassword).Result)
        {
            userManager.RemovePasswordAsync(admin).Wait();
            var result = userManager.AddPasswordAsync(admin, adminPassword).Result;

            if (!result.Succeeded) throw new Exception($"Failed to change admin profile password: {result.Errors}");
        }

        // Перевірити що профіль адміна має роль "адмін"
        if (!userManager.IsInRoleAsync(admin, Constants.AdminRoleName).Result)
        {
            var result = userManager.AddToRoleAsync(admin, adminRole.Name).Result;

            if (!result.Succeeded) throw new Exception($"Failed to add \"{adminRole.Name}\" role to admin profile: {result.Errors}");
        }

        return app;
    }
}