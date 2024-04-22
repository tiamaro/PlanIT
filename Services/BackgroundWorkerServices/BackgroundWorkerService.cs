using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Services.MailService;

public class BackgroundWorkerService : IHostedService, IDisposable
{
    private readonly ILogger<BackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer;
    //private readonly PlanITDbContext _dbContext;
    // private readonly IMailService _mailService;

    public BackgroundWorkerService(
        ILogger<BackgroundService> logger,
        //IMailService mailService,
        IServiceProvider serviceProvider)
        //PlanITDbContext dbContext)
    {
        _logger = logger;
        //_dbContext = dbContext;
        //_mailService = mailService;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background service started");

        // Set up a timer to execute DoWork periodically
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        try
        {
            // Create a scope for this background task
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PlanITDbContext>();
                var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                // Get invites that need reminders
                //var invitesToCheck = dbContext.Invites
                //    .Where(x => !x.IsReminderSent && x.Event.Date <= today.AddDays(3))
                //    .ToList();

                var invitesToCheck = dbContext.Invites
                .Include(i => i.Event)
                .ThenInclude(e => e!.User)  // Include User data linked through Event
                .Where(x => !x.IsReminderSent && x.Event!.Date <= today.AddDays(3))
                .ToList();

                foreach (var invite in invitesToCheck)
                {
                    // Send reminder email
                    mailService.SendReminderEmail(invite);

                    // Mark the invite as sent
                    invite.IsReminderSent = true;
                    dbContext.Update(invite);
                }

                // Save changes within the scope
                dbContext.SaveChanges();
            }

            _logger.LogInformation($"Found email invites with IsReminderSent=false");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in background service");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background service stopped");
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
