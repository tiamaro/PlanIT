namespace PlanIT.API.Utilities;

public class LoggerServiceFactory : ILoggerServiceFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public LoggerServiceFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public LoggerService CreateLogger()
    {
        var scope = _scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<LoggerService>();
    }
}
