using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PlanIT.API.Middleware;

// Exception Handling:
// - HandleExceptionFilter: Dette filteret er tilknyttet kontrollere for å fange og behandle
//   unntak på en sentralisert måte. Det sikrer at alle uventede feil eller unntak håndteres
//   på en konsekvent måte, og at en passende feilmelding sendes tilbake til klienten.

public class HandleExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<HandleExceptionFilter> _logger;

    public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An error occurred in {Controller} at {Action}",
            context.RouteData.Values["controller"], context.RouteData.Values["action"]);

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

        context.Result = result;
        context.ExceptionHandled = true;
    }
}