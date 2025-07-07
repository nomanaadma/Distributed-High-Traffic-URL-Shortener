using URL_Shortener.Core;
using URL_Shortener.Core.Urls.Add;

namespace URL_Shortener.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlFeature(this IServiceCollection services)
    {
        services.AddScoped<AddUrlHandler>();
        services.AddSingleton<TokenProvider>();
        services.AddScoped<ShortUrlGenerator>();

        return services;
    }
}

