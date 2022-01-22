using Serilog.Events;

namespace CarvedRock.Pipeline.Api.Middleware
{
    public static class LogHelper
    {
        public static LogEventLevel CustomGetLevel(HttpContext ctx, double _, Exception ex) =>
            ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : LogEventLevel.Debug; //Debug instead of Information
    }
}
