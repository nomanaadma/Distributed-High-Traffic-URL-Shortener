using Azure.Identity;
using URL_Shortener.Api.Extensions;
using URL_Shortener.Core.Urls.Add;
using URL_Shortener.Infrastructure.Extensions;
using Url_Shortener.Api;

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
    .AddCosmosUrlDataStore(builder.Configuration);

builder.Services.AddSingleton<IEnvironmentManager, EnvironmentManager>();
builder.Services.AddSingleton<ITokenRangeApiClient, TokenRangeApiClient>();

builder.Services.AddHttpClient("TokenRangeService",
    client =>
    {
        client.BaseAddress = 
            new Uri(builder.Configuration["TokenRangeService:Endpoint"]!);
    });

builder.Services.AddHostedService<TokenManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "APIs");

app.MapPost("/api/urls",
    async (AddUrlHandler handler,
        AddUrlRequest request,
        CancellationToken cancellationToken) =>
    {
        var requestWithUser = request with
        {
            CreatedBy = "nomanaadma@gmail.com"
        };
        
        var result = await handler.HandleAsync(requestWithUser, cancellationToken);

        if (!result.Succeeded)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Created($"/api/urls/{result.Value!.ShortUrl}",
            result.Value);
    });


app.Run();

