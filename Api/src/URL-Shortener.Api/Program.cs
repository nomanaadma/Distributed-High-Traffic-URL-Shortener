using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using URL_Shortener.Api.Extensions;
using URL_Shortener.Core.Urls.Add;
using URL_Shortener.Infrastructure.Extensions;
using Url_Shortener.Api;
using Url_Shortener.Api.Extensions;
using URL_Shortener.Core.Urls;
using URL_Shortener.Core.Urls.Delete;
using URL_Shortener.Core.Urls.List;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];

if (!string.IsNullOrEmpty(keyVaultName))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services
    .AddUrlFeature()
    .AddListUrlsFeature()
    .AddCosmosUrlDataStore(builder.Configuration);

builder.Services.AddSingleton<IEnvironmentManager, EnvironmentManager>();
builder.Services.AddSingleton<ITokenRangeApiClient, TokenRangeApiClient>();

builder.Services.AddSingleton(
	new RedirectLinkBuilder(
		new Uri(builder.Configuration["RedirectService:Endpoint"]!)));

builder.Services.AddHttpClient("TokenRangeService",
    client =>
    {
        client.BaseAddress = 
            new Uri(builder.Configuration["TokenRangeService:Endpoint"]!);
    });

builder.Services.AddHostedService<TokenManager>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(options =>
		{
			builder.Configuration.Bind("AzureAd", options);
			options.TokenValidationParameters.NameClaimType = "name";
		},
		options => { builder.Configuration.Bind("AzureAd", options); });

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("AuthZPolicy", policyBuilder =>
		policyBuilder.Requirements.Add(new ScopeAuthorizationRequirement()
		{
			RequiredScopesConfigurationKey = "AzureAd:Scopes"
		}));

builder.Services.AddAuthorization(options =>
{
	options.DefaultPolicy =
		new AuthorizationPolicyBuilder(
				JwtBearerDefaults.AuthenticationScheme)
			.RequireAuthenticatedUser()
			.Build();
	// By default, all incoming requests will be authorized according to 
	// the default policy    
	options.FallbackPolicy = options.DefaultPolicy;
});


builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowWebApp", policy =>
	{
		if(builder.Configuration["WebAppEndpoints"] is null)
			return;
        
		var origins = builder.Configuration["WebAppEndpoints"]!.Split(",");
        
		policy
			.WithOrigins(origins.ToArray())
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "API")
	.AllowAnonymous();

app.MapPost("/api/urls",
    async (AddUrlHandler handler,
        AddUrlRequest request,
        HttpContext context,
        CancellationToken cancellationToken) =>
    {
	    var email = context.User.GetUserEmail();
	    
        var requestWithUser = request with
        {
            CreatedBy = email
        };
        
        var result = await handler.HandleAsync(requestWithUser, cancellationToken);

        if (!result.Succeeded)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Created($"/api/urls/{result.Value!.ShortUrl}",
            result.Value);
    });


app.MapGet("/api/urls", async (HttpContext context,
		ListUrlsHandler handler,
		int? pageSize,
		[FromQuery(Name = "continuation")] string? continuationToken,
		CancellationToken cancellationToken) =>
	{
		var request = new ListUrlsRequest(context.User.GetUserEmail(), pageSize,
			continuationToken);
		var urls = await handler.HandleAsync(request, cancellationToken);

		return urls;
	}
);

app.MapDelete("/api/urls/{shortUrl}", async (string shortUrl, 
		HttpContext context, 
		DeleteUrlHandler handler,
		CancellationToken cancellationToken) =>
	{
		var request = new DeleteUrlRequest(shortUrl, context.User.GetUserEmail());
		var result = await handler.HandleAsync(request, cancellationToken);
		
		if (!result.Succeeded)
		{
			return Results.BadRequest(result.Error);
		}
		
		return result.Value == DeleteUrlResponse.Deleted ? Results.NoContent() : Results.NotFound();
	}
);

app.Run();

