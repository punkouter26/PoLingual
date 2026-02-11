using Azure;
using Azure.Data.Tables;

namespace PoLingual.Shared.Models;

/// <summary>
/// Rapper entity for Azure Table Storage.
/// </summary>
public class Rapper : ITableEntity
{
    public string Name { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int TotalDebates { get; set; }

    // ITableEntity Implementation
    public string PartitionKey { get; set; } = "Rapper";
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public Rapper() { }

    public Rapper(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Rapper name cannot be empty.", nameof(name));

        Name = name;
        RowKey = name;
        PartitionKey = "Rapper";
        Wins = 0;
        Losses = 0;
        TotalDebates = 0;
    }
}
