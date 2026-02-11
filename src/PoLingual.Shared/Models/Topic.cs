namespace PoLingual.Shared.Models;

/// <summary>
/// Simple topic data transfer object.
/// </summary>
public class Topic
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
