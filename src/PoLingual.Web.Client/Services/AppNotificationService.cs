namespace PoLingual.Web.Client.Services;

/// <summary>
/// Client-side notification service for managing app-wide notifications.
/// </summary>
public class AppNotificationService
{
    public event Action<string, string>? OnNotification;

    public void ShowNotification(string title, string message)
    {
        OnNotification?.Invoke(title, message);
    }
}
