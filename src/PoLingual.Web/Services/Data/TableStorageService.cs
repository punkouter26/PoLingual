using Azure.Data.Tables;

namespace PoLingual.Web.Services.Data;

/// <summary>
/// Generic Azure Table Storage service using Repository pattern abstraction.
/// </summary>
public class TableStorageService : ITableStorageService
{
    private readonly string _connectionString;
    private readonly ILogger<TableStorageService> _logger;

    public TableStorageService(IConfiguration configuration, ILogger<TableStorageService> logger)
    {
        _connectionString = configuration["Azure:StorageConnectionString"]
            ?? throw new InvalidOperationException("Azure:StorageConnectionString not found in configuration.");
        _logger = logger;
    }

    private TableServiceClient GetTableServiceClient() => new TableServiceClient(_connectionString);

    public async Task<TableClient> GetTableClientAsync(string tableName)
    {
        var tableClient = GetTableServiceClient().GetTableClient(tableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }

    public async Task<T?> GetEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity
    {
        try
        {
            var tableClient = await GetTableClientAsync(tableName);
            return await tableClient.GetEntityAsync<T>(partitionKey, rowKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity from {Table} PK={PK} RK={RK}", tableName, partitionKey, rowKey);
            return null;
        }
    }

    public async IAsyncEnumerable<T> GetEntitiesAsync<T>(string tableName, string? filter = null) where T : class, ITableEntity
    {
        var tableClient = await GetTableClientAsync(tableName);
        await foreach (var entity in tableClient.QueryAsync<T>(filter))
            yield return entity;
    }

    public async Task AddEntityAsync<T>(string tableName, T entity) where T : class, ITableEntity
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.AddEntityAsync(entity);
    }

    public async Task UpsertEntityAsync<T>(string tableName, T entity) where T : class, ITableEntity
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.UpsertEntityAsync(entity);
    }

    public async Task DeleteEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : class, ITableEntity
    {
        var tableClient = await GetTableClientAsync(tableName);
        await tableClient.DeleteEntityAsync(partitionKey, rowKey);
    }
}
