﻿using Serilog.Events;

namespace CarvedRock.Pipeline.Api.Middleware
{
    // pjm - copied from:
    //https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-excluding-health-check-endpoints-from-serilog-request-logging/

    public static class LogHelper
    {
        //https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-excluding-health-check-endpoints-from-serilog-request-logging/
        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var endpoint = ctx.GetEndpoint();
            if (endpoint is object) // same as !(endpoint is null)
            {
                return string.Equals(
                    endpoint.DisplayName,
                    "Health checks",
                    StringComparison.Ordinal);
            }
            // No endpoint, so not a health check endpoint
            return false;
        }
        public static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex) => ex != null
        ? LogEventLevel.Error
        : ctx.Response.StatusCode > 499
            ? LogEventLevel.Error
            : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                ? LogEventLevel.Verbose // Was a health check, use Verbose
                : LogEventLevel.Information;
    }
}
