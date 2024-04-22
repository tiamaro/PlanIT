using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Services.MailService;
using PlanIT.API.Utilities;

public class BackgroundWorkerService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundWorkerService> _logger;
    private Timer? _timer;

    public BackgroundWorkerService(IServiceProvider serviceProvider, ILogger<BackgroundWorkerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background service started");
        // Initialize the timer
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PlanITDbContext>();
        var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();
        var loggerService = scope.ServiceProvider.GetRequiredService<LoggerService>();

        try
        {
            var invitesToCheck = await FetchInvites(dbContext);
            await ProcessInvites(invitesToCheck, mailService, dbContext, loggerService);
        }
        catch (Exception ex)
        {
            loggerService.LogException(ex, "Error during the background work execution.");
        }
    }

    private async Task<List<Invite>> FetchInvites(PlanITDbContext dbContext)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await dbContext.Invites
            .Include(i => i.Event)
            .ThenInclude(e => e!.User)
            .Where(x => !x.IsReminderSent && x.Event!.Date <= today.AddDays(3))
            .ToListAsync();
    }

    private async Task ProcessInvites(List<Invite> invites, IMailService mailService, PlanITDbContext dbContext, LoggerService loggerService)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            foreach (var invite in invites)
            {
                if (await TrySendReminder(invite, mailService, loggerService))
                {
                    invite.IsReminderSent = true;
                    dbContext.Update(invite);
                }
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            loggerService.LogInfo($"Transaction committed. Processed {invites.Count} invites.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            loggerService.LogException(ex, "Transaction rolled back due to an error.");
            throw; // Ensure to rethrow to maintain error visibility
        }
    }

    private async Task<bool> TrySendReminder(Invite invite, IMailService mailService, LoggerService loggerService)
    {
        try
        {
            await mailService.SendReminderEmail(invite);
            return true;
        }
        catch (Exception ex)
        {
            loggerService.LogException(ex, $"Failed to send reminder for invite ID {invite.Id}");
            return false;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background service stopping");
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
