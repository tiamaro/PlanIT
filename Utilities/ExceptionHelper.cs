namespace PlanIT.API.Utilities;
// Provides utility methods to create specific types of exceptions with standardized messages.

public static class ExceptionHelper
{
    // Creates a new KeyNotFoundException with a formatted message indicating an entity was not found.
    public static Exception CreateNotFoundException(string entity, int id)
        => new KeyNotFoundException($"{entity} with ID {id} not found.");


    // Creates a new UnauthorizedAccessException with a formatted message indicating denied access.
    public static Exception CreateUnauthorizedException(string entity, int id)
        => new UnauthorizedAccessException($"Access denied for {entity} ID {id}.");


    // Creates a new InvalidOperationException with a formatted message indicating a failure in operation.
    public static Exception CreateOperationException(string entity, int id, string operation)
        => new InvalidOperationException($"Failed to {operation} {entity} with ID {id}.");
}