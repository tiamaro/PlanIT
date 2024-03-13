﻿namespace PlanIT.API.Middleware;

// Interface IMiddelware -> innebygd
public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    // Lager konstruktør med Logger
    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong - test exception {@Machine} {@TraceId}",
               Environment.MachineName,
               System.Diagnostics.Activity.Current?.Id);

            await Results.Problem(
                title: "GlobalException has discovered a big problem!!",
                statusCode: StatusCodes.Status500InternalServerError,
                extensions: new Dictionary<string, Object?>
                {
                    { "traceId", System.Diagnostics.Activity.Current?.Id },

                }).ExecuteAsync(context);
        }
    }
}
