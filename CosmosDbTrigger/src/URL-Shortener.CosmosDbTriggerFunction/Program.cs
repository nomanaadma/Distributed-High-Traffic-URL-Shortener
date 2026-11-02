using Azure.Identity;
using HealthChecks.CosmosDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton<CosmosClient>(s =>
    new CosmosClient(
        connectionString: builder.Configuration["CosmosDb:ConnectionString"]
    ));

builder.Services.AddSingleton<Container>(s =>
{
    var client = s.GetRequiredService<CosmosClient>();
    return client.GetContainer(
        builder.Configuration["TargetDatabaseName"], 
        builder.Configuration["TargetContainerName"]);
});

builder.Build().Run();
