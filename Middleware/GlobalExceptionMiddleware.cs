namespace PlanIT.API.Middleware;

// Middleware for global exception handling in the application pipeline.
public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    
    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }


    
    // Invokes the middleware operation.
    // Returns a Task representing the asynchronous operation of middleware invocation.
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            // Proceed with the next middleware in the pipeline
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong - test exception {@Machine} {@TraceId}",
               Environment.MachineName,
               System.Diagnostics.Activity.Current?.Id);


            // Respond to the request with a problem details object, setting a 500 internal server error status
            await Results.Problem(
                title: "GlobalException has discovered a problem.",
                statusCode: StatusCodes.Status500InternalServerError,
                extensions: new Dictionary<string, Object?>
                {
                    { "traceId", System.Diagnostics.Activity.Current?.Id },

                }).ExecuteAsync(context);
        }
    }
}
