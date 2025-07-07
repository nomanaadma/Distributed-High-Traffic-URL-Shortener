using Azure.Identity;
using URL_Shortener.TokenRangeService;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];

if (!string.IsNullOrEmpty(keyVaultName))
{
	builder.Configuration.AddAzureKeyVault(
		new Uri($"https://{keyVaultName}.vault.azure.net/"),
		new DefaultAzureCredential());
}

builder.Services.AddSingleton(
	new TokenRangeManager(builder.Configuration["Postgres:ConnectionString"]!));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "TokenRange Service");

app.MapPost("/assign", async (AssignTokenRangeRequest request, TokenRangeManager manager) =>
{
	var range = await manager.AssignRangeAsync(request.Key);
	return range;
});

app.Run();






