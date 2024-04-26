namespace PlanIT.API.Utilities;
// Factory for creating LoggerService instances.

public class LoggerServiceFactory : ILoggerServiceFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public LoggerServiceFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    // Creates a new LoggerService instance using dependency injection
    public LoggerService CreateLogger()
    {
        // Create a new dependency injection scope.
        var scope = _scopeFactory.CreateScope();

        // Resolve and return the LoggerService from the new scope.
        return scope.ServiceProvider.GetRequiredService<LoggerService>();
    }
}
