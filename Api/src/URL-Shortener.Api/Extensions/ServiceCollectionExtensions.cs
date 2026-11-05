using URL_Shortener.Core;
using URL_Shortener.Core.Urls.Add;
using URL_Shortener.Core.Urls.Delete;
using URL_Shortener.Core.Urls.List;

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
    
    public static IServiceCollection AddListUrlsFeature(this IServiceCollection services)
    {
	    services.AddScoped<ListUrlsHandler>();

	    return services;
    }
    
    public static IServiceCollection AddDeleteUrlFeature(this IServiceCollection services)
    {
	    services.AddScoped<DeleteUrlHandler>();

	    return services;
    }
    
}

