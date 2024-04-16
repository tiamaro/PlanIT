namespace PlanIT.API.Utilities;

public static class ExceptionHelper
{
    public static Exception CreateNotFoundException(string entity, int id)
        => new KeyNotFoundException($"{entity} with ID {id} not found.");

    public static Exception CreateUnauthorizedException(string entity, int id)
        => new UnauthorizedAccessException($"Access denied for {entity} ID {id}.");

    public static Exception CreateOperationException(string entity, int id, string operation)
        => new InvalidOperationException($"Failed to {operation} {entity} with ID {id}.");
}