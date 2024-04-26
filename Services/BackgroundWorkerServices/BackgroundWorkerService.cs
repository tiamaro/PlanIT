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

    // Starts the background service by setting up a timer.
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var logger = _loggerFactory.CreateLogger();
        logger.LogInfo("Background service started");

        // Set the timer to trigger the DoWork method every 10 minutes.
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        return Task.CompletedTask;
    }

    // The work performed by the timer. Manages database operations and email notifications.
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

    // Fetches upcoming invites that need reminders sent.
    private async Task<List<Invite>> FetchInvites(PlanITDbContext dbContext)
    {
        // Define today's date for comparison and filter invites that need reminders within the next three days.
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await dbContext.Invites
            .Include(i => i.Event)
            .ThenInclude(e => e!.User)
            .Where(x => !x.IsReminderSent && x.Event!.Date <= today.AddDays(3))
            .ToListAsync();
    }

    // Processes each invite, sending reminders and updating the database.
    private async Task ProcessInvites(List<Invite> invites, IMailService mailService, PlanITDbContext dbContext, LoggerService logger)
    {

        // Begin a transaction to ensure data consistency during update operations.
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

            // Save changes to the database and commit the transaction if all updates were successful.
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            logger.LogInfo($"Transaction committed. Processed {invites.Count} invites.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogException(ex, "Transaction rolled back due to an error.");

            // Rethrow to ensure the error is handled upstream.
            throw; 
        }
    }

    // Attempts to send an email reminder for an invite.
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

    // Stops the background service and disposes the timer.
    public Task StopAsync(CancellationToken cancellationToken)
    {
        var logger = _loggerFactory.CreateLogger();
        logger.LogInfo("Background service stopping");
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    // Disposes resources used by the background service.
    public void Dispose()
    {
        _timer?.Dispose();
    }
}
