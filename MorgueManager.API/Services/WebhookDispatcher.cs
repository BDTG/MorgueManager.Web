using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MorgueManager.API.Services;

public class WebhookDispatcher
{
    private readonly ILogger<WebhookDispatcher> _logger;

    public WebhookDispatcher(ILogger<WebhookDispatcher> logger)
    {
        _logger = logger;
    }

    public async Task SendWebhookAlertAsync(string alertType, string message)
    {
        try
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            var filePath = Path.Combine(logDir, "webhook-alerts.txt");
            var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            var logLine = $"[{timestamp}] [{alertType.ToUpper()}] {message}{Environment.NewLine}";

            // Append warning / error details to the simulated webhook alerts log file
            await File.AppendAllTextAsync(filePath, logLine);

            _logger.LogInformation("Webhook alert [{AlertType}] simulated successfully to Logs/webhook-alerts.txt", alertType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to simulate sending webhook alert.");
        }
    }
}
