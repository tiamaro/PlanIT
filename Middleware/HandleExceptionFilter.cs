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

        context.Result = new ObjectResult(new
        {
            status = StatusCodes.Status500InternalServerError,
            message = "An internal error occurred, please try again."
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}