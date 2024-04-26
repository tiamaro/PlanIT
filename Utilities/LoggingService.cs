namespace PlanIT.API.Utilities;

// Provides centralized logging capabilities across the application
// logs various types of messages including informational, debug, warning, and error
// messages, along with structured logs for specific operational contexts such as creation,
// deletion, and access control.

public class LoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }

    public void LogInfo(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public void LogException(Exception ex, string message, params object[] args)
    {
        _logger.LogError(ex, message, args);
    }

    // Spesifiserte logging metoder for standard oppgaver
    public void LogCreationStart(string entity)
    {
        _logger.LogInformation($"Starting to create a new {entity}.");
    }

    public void LogCreationFailure(string entity)
    {
        _logger.LogWarning($"Failed to create new {entity}.");
    }

    public void LogNotFound(string entity, int entityId)
    {
        _logger.LogWarning($"{entity} with ID {entityId} not found.");
    }

    public void LogUnauthorizedAccess(string entity, int entityId, int userId)
    {
        _logger.LogWarning($"Unauthorized attempt to access {entity} with ID {entityId} by user ID {userId}.");
    }

    public void LogOperationSuccess(string operation, string entity, int entityId)
    {
        _logger.LogInformation($"{entity} with ID {entityId} {operation} successfully.");
    }

    public void LogOperationFailure(string operation, string entity, int entityId)
    {
        _logger.LogError($"Failed to {operation} {entity} with ID {entityId}.");
    }
}