namespace PoLingual.Shared.Models;

public class DiagnosticResult
{
    public DateTime Timestamp { get; set; }
    public bool IsHealthy { get; set; }
    public Dictionary<string, string> Checks { get; set; } = new Dictionary<string, string>();
    public string CheckName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
