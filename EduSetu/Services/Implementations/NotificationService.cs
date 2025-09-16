using EduSetu.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace EduSetu.Services.Implementations;

public class NotificationService : INotificationService
{
    public event Action<NotificationMessage>? OnNotify;

    public void Success(string message)
    {
        Notify(message, "success");
    }

    public void Error(string message)
    {
        Notify(message, "error");
    }

    public void Warning(string message)
    {
        Notify(message, "warning");
    }

    private void Notify(string message, string type)
    {
        OnNotify?.Invoke(new NotificationMessage
        {
            Message = message,
            Type = type
        });
    }
}

public class NotificationMessage
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "success"; // success, error, warning
}