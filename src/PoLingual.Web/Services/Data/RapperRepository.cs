using Azure.Data.Tables;
using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.Data;

/// <summary>
/// Repository Pattern for Rapper entities in Azure Table Storage.
/// </summary>
public class RapperRepository : IRapperRepository
{
    private const string TableName = "PoLingualRappers";
    private readonly ITableStorageService _tableStorageService;
    private readonly ILogger<RapperRepository> _logger;

    public RapperRepository(ITableStorageService tableStorageService, ILogger<RapperRepository> logger)
    {
        _tableStorageService = tableStorageService;
        _logger = logger;
    }

    public async Task<List<Rapper>> GetAllRappersAsync()
    {
        var rappers = new List<Rapper>();
        await foreach (var entity in _tableStorageService.GetEntitiesAsync<RapperEntity>(TableName))
        {
            rappers.Add(new Rapper { Name = entity.RowKey, Wins = entity.Wins, Losses = entity.Losses });
        }
        _logger.LogInformation("Retrieved {Count} rappers from Table Storage", rappers.Count);
        return rappers;
    }

    public async Task SeedInitialRappersAsync()
    {
        var existing = await GetAllRappersAsync();
        if (existing.Count > 0) { _logger.LogInformation("Rappers already exist. Skipping seeding."); return; }

        _logger.LogInformation("Seeding initial rappers...");
        var rappers = new[] { "Eminem", "Kendrick Lamar", "Tupac Shakur", "The Notorious B.I.G.", "Nas",
                              "Jay-Z", "Rakim", "Andre 3000", "Lauryn Hill", "Snoop Dogg" };

        foreach (var name in rappers)
            await _tableStorageService.UpsertEntityAsync(TableName, new RapperEntity(TableName, name));

        _logger.LogInformation("Initial rappers seeded successfully.");
    }

    public async Task UpdateWinLossRecordAsync(string winnerName, string loserName)
    {
        _logger.LogInformation("Updating record: winner={Winner}, loser={Loser}", winnerName, loserName);

        var winner = await _tableStorageService.GetEntityAsync<RapperEntity>(TableName, TableName, winnerName);
        if (winner != null) { winner.Wins++; await _tableStorageService.UpsertEntityAsync(TableName, winner); }
        else await _tableStorageService.UpsertEntityAsync(TableName, new RapperEntity(TableName, winnerName) { Wins = 1 });

        var loser = await _tableStorageService.GetEntityAsync<RapperEntity>(TableName, TableName, loserName);
        if (loser != null) { loser.Losses++; await _tableStorageService.UpsertEntityAsync(TableName, loser); }
        else await _tableStorageService.UpsertEntityAsync(TableName, new RapperEntity(TableName, loserName) { Losses = 1 });
    }

    /// <summary>Table entity for internal storage representation.</summary>
    public class RapperEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public Azure.ETag ETag { get; set; }
        public RapperEntity() { }
        public RapperEntity(string partitionKey, string rowKey) { PartitionKey = partitionKey; RowKey = rowKey; }
    }
}
