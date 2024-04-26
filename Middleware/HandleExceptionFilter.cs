using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlanIT.API.Middleware;

// Exception Handling:
// Exception filter that captures and handles exceptions globally across controllers.
// This ensures that all unhandled exceptions are processed consistently and that appropriate
// error responses are sent back to the client.

public class HandleExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<HandleExceptionFilter> _logger;

    public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger)
    {
        _logger = logger;
    }

    // Called when an exception occurs during action execution.
    // Logs the error and creates a corresponding response based on the exception type.
    public override void OnException(ExceptionContext context)
    {
        // Log the error including the controller and action where it occurred
        _logger.LogError(context.Exception, "An error occurred in {Controller} at {Action}",
            context.RouteData.Values["controller"], context.RouteData.Values["action"]);


        // Determine the type of response based on the exception type
        ObjectResult result;

        switch (context.Exception)
        {
            case ArgumentException ex:
                result = new ObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message ?? "Invalid parameters."
                })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
                break;
            case KeyNotFoundException ex:
                result = new ObjectResult(new
                {
                    status = StatusCodes.Status404NotFound,
                    message = ex.Message ?? "The requested resource was not found."
                })
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                break;
            case UnauthorizedAccessException ex:
                result = new ObjectResult(new
                {
                    status = StatusCodes.Status403Forbidden,
                    message = ex.Message ?? "You do not have permission to perform this action."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                break;
            case InvalidOperationException ex:
                result = new ObjectResult(new
                {
                    status = StatusCodes.Status409Conflict,
                    message = ex.Message ?? "The operation was invalid."
                })
                {
                    StatusCode = StatusCodes.Status409Conflict
                };
                break;
            default:
                result = new ObjectResult(new
                {
                    status = StatusCodes.Status500InternalServerError,
                    message = context.Exception.Message ?? "An internal error occurred, please try again."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                break;
        }

        // Set the result of the context to the created ObjectResult
        context.Result = result;

        // Mark the exception as handled to prevent further propagation
        context.ExceptionHandled = true;
    }
}