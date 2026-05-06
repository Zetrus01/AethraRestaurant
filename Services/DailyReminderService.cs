using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DailyReminderService : BackgroundService
{
    private readonly ILogger<DailyReminderService> _logger;
    private readonly IServiceProvider _services;

    public DailyReminderService(
        ILogger<DailyReminderService> logger,
        IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Napi emlékeztető szolgáltatás elindult");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Következő futás: reggel 8:00
            var now = DateTime.Now;
            var nextRun = now.Date.AddHours(21);
            
            if (now >= nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }
            
            var waitTime = nextRun - now;
            _logger.LogInformation($"📅 Következő emlékeztető küldés: {nextRun:yyyy-MM-dd HH:mm:ss} ({(int)waitTime.TotalHours} óra múlva)");
            
            await Task.Delay(waitTime, stoppingToken);
            
            await SendTodayRemindersAsync();
        }
    }

    private async Task SendTodayRemindersAsync()
    {
        try
        {
            _logger.LogInformation("📧 Napi emlékeztetők küldésének indítása...");
            
            using var scope = _services.CreateScope();
            var controller = scope.ServiceProvider.GetRequiredService<ReservationController>();
            
            var result = await controller.SendTodayReservationReminders(isAutomated: true);
            
            _logger.LogInformation("✅ Napi emlékeztetők elküldve");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Hiba a napi emlékeztetők küldésekor");
        }
    }
}