using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services.BackgroundWorkerServices;

public class BackgroundWorkerService : IHostedService, IDisposable
{
    private readonly ILogger<BackgroundService> _logger;
    private Timer _timer;
    private readonly PlanITDbContext _dbContext;
    private readonly IMailService _mailService;

    public BackgroundWorkerService(ILogger<BackgroundService> logger, Timer timer, PlanITDbContext dbcontext, IMailService mailservice)
    {
        _logger = logger;
        _timer = timer;
        _dbContext = dbcontext;
        _mailService = mailservice;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background service started");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));


        return Task.CompletedTask;
        
    }

    

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background service stopping");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    // async Task ?? 
    private async void DoWork(object? state)
    {
        try
        {
            var invitesToCheck = _dbContext.Invites.Where(x => !x.IsReminderSent).ToList();

            foreach(var invite in invitesToCheck)
            {
                await _mailService.SendReminderEmail(invite);
                invite.IsReminderSent = true;
                _dbContext.Update(invite);
                await _dbContext.SaveChangesAsync();
                
            }


            _logger.LogInformation($"Found {invitesToCheck.Count} invites with IsReminderSent=false");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in background service");
        }
    }

}
