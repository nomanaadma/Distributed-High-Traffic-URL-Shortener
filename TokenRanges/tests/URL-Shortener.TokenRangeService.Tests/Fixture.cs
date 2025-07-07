using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;

namespace URL_Shortener.TokenRangeService.Tests;

public class Fixture : WebApplicationFactory<ITokenRangeAssemblyMarker>, IAsyncLifetime
{
	private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
		.Build();
	private string ConnectionString => _postgresContainer.GetConnectionString();

	public async Task InitializeAsync()
	{
		await _postgresContainer.StartAsync();
        
		Environment.SetEnvironmentVariable("Postgres__ConnectionString", ConnectionString);
        
		await InitializeSqlTable();
	}
	
	private async Task InitializeSqlTable()
	{
		var tableSql = await File.ReadAllTextAsync(
			"Table.sql");
        
		await using var connection = new NpgsqlConnection(ConnectionString);
		await connection.OpenAsync();
        
		await using var command = new NpgsqlCommand(tableSql, connection);
		await command.ExecuteNonQueryAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await _postgresContainer.StopAsync();
		await base.DisposeAsync();
		GC.SuppressFinalize(this);
	}
	
	async Task IAsyncLifetime.DisposeAsync()
	{
		await DisposeAsync();
	}
	
}