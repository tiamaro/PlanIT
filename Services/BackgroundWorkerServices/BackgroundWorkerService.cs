using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Services.MailService;
using PlanIT.API.Utilities;


public class BackgroundWorkerService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerServiceFactory _loggerFactory;
    private Timer? _timer;

    public BackgroundWorkerService(IServiceProvider serviceProvider, ILoggerServiceFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var logger = _loggerFactory.CreateLogger();
        logger.LogInfo("Background service started");
        // Initialize the timer
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        var logger = _loggerFactory.CreateLogger();
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PlanITDbContext>();
        var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

        try
        {
            var invitesToCheck = await FetchInvites(dbContext);
            await ProcessInvites(invitesToCheck, mailService, dbContext, logger);
        }
        catch (Exception ex)
        {
            logger.LogException(ex, "Error during the background work execution.");
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

    private async Task ProcessInvites(List<Invite> invites, IMailService mailService, PlanITDbContext dbContext, LoggerService logger)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            foreach (var invite in invites)
            {
                if (await TrySendReminder(invite, mailService, logger))
                {
                    invite.IsReminderSent = true;
                    dbContext.Update(invite);
                }
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            logger.LogInfo($"Transaction committed. Processed {invites.Count} invites.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogException(ex, "Transaction rolled back due to an error.");
            throw; // Ensure to rethrow to maintain error visibility
        }
    }

    private async Task<bool> TrySendReminder(Invite invite, IMailService mailService, LoggerService logger)
    {
        try
        {
            await mailService.SendReminderEmail(invite);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogException(ex, $"Failed to send reminder for invite ID {invite.Id}");
            return false;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var logger = _loggerFactory.CreateLogger();
        logger.LogInfo("Background service stopping");
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
