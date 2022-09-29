using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UrlShortener.BLL.CustomServices;
using UrlShortener.BLL.Jobs;

namespace UrlShortener.BLL;

public static class AspNetCoreExtensions
{
    /// <summary>
    /// Додати сервіси до DI контейнеру.
    /// </summary>
    /// <param name="builder">Білдер додатка.</param>
    /// <returns>Білдер додатка для подальшого налаштування.</returns>
    public static WebApplicationBuilder AddAppDependencyInjection(this WebApplicationBuilder builder)
    {
        // Сервіси, що реалізують інтерфейс IEntityService
        builder.Services.AddScoped<IEntityService<ShortenedUrl, string>, ShortenedUrlService>();

        // Сервіси, що реалізують інтерфейс IEFEntityService
        builder.Services.AddScoped<IEFEntityService<ShortenedUrl, string>, ShortenedUrlService>();

        // Сервіси з власною логікою 
        builder.Services.AddSingleton<RangeProviderService>();
        builder.Services.AddTransient<HashGeneratorService>();
        builder.Services.AddScoped<ShortenedUrlService>();

        // Джоби
        builder.Services.AddSingleton<IHostedService, CleanupJob>();

        return builder;
    }
}