namespace PlanIT.API.Utilities;

public class LoggerService
{
    private readonly Serilog.ILogger _logger;  // Bruker Serilog

    public LoggerService(Serilog.ILogger logger)
    {
        _logger = logger;
    }

    public void LogInfo(string message, params object[] args)
    {
        _logger.Information(message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.Debug(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.Warning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.Error(message, args);
    }

    public void LogException(Exception ex, string message, params object[] args)
    {
        _logger.Error(ex, message, args);
    }

    // Spesifikke loggmetoder for standardoppgaver
    public void LogCreationStart(string entity)
    {
        _logger.Information($"Starting to create a new {entity}.");
    }

    public void LogCreationFailure(string entity)
    {
        _logger.Warning($"Failed to create new {entity}.");
    }

    public void LogNotFound(string entity, int entityId)
    {
        _logger.Warning($"{entity} with ID {entityId} not found.");
    }

    public void LogUnauthorizedAccess(string entity, int entityId, int userId)
    {
        _logger.Warning($"Unauthorized attempt to access {entity} with ID {entityId} by user ID {userId}.");
    }

    public void LogOperationSuccess(string operation, string entity, int entityId)
    {
        _logger.Information($"{entity} with ID {entityId} {operation} successfully.");
    }

    public void LogOperationFailure(string operation, string entity, int entityId)
    {
        _logger.Error($"Failed to {operation} {entity} with ID {entityId}.");
    }
}