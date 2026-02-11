using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.Diagnostics;

public interface IDiagnosticsService
{
    Task<List<DiagnosticResult>> RunAllChecksAsync();
}
