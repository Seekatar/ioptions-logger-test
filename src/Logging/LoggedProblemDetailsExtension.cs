using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IOptionTest.Logging;

static class LoggedProblemDetailsExtension
{
    public static ProblemDetailsException LogProblemDetails(this ILogger logger, LogLevel logLevel, string title, string detail, params object?[]? args)
    {
        return logger.LogProblemDetails(logLevel, title, null, detail, args);
    }

    public static ProblemDetailsException LogProblemDetails(this ILogger logger, LogLevel logLevel, ProblemDetails details)
    {
        var e = new ProblemDetailsException(details);
        logger.LogException(logLevel, e);
        return e;
    }

    public static ProblemDetailsException LogProblemDetails(this ILogger logger, LogLevel logLevel, string title, Exception? innerException, string detail, params object?[]? args)
    {
        var e = new ProblemDetails
        {
            Title = title,
            Status = (int)HttpStatusCode.BadRequest,
            Detail = args is null ? detail : string.Format(detail, args),
        };
        return logger.LogProblemDetails(logLevel, e);
    }

    public static void LogException(this ILogger logger, LogLevel logLevel, ProblemDetailsException e, params object?[]? args)
    {
        using var scope = logger.BeginScope(e.Details.Extensions);
        logger.Log(logLevel, e, e.Details.Detail, args!);

        if (e.InnerException is not null)
            logger.Log(logLevel, e.InnerException, "Inner exception");
    }
}
#pragma warning restore CA2254