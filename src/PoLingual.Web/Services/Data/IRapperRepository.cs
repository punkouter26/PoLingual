using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.Data;

public interface IRapperRepository
{
    Task<List<Rapper>> GetAllRappersAsync();
    Task SeedInitialRappersAsync();
    Task UpdateWinLossRecordAsync(string winnerName, string loserName);
}
