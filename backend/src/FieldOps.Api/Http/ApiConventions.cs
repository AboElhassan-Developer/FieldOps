using System.Text.Json.Serialization;
using FieldOps.SharedKernel.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace FieldOps.Api.Http;

/// <summary>HTTP conventions applied to every endpoint: routing, JSON, validation and error responses.</summary>
internal static class ApiConventions
{
    public static IServiceCollection AddApiConventions(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);

        services
            .AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
                options.Filters.Add<ValidationFilter>();
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Every error response, including unhandled exceptions and empty 404/405 responses,
        // is written as ProblemDetails (RFC 9457) with a stable "code" and the request "traceId".
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            var problem = context.ProblemDetails;
            if (!problem.Extensions.ContainsKey(ErrorCodes.ExtensionKey))
            {
                problem.Extensions[ErrorCodes.ExtensionKey] = problem is HttpValidationProblemDetails
                    ? ErrorCodes.ValidationFailed
                    : ErrorCodeFor(problem.Status);
            }
        });

        return services;
    }

    public static WebApplication UseApiConventions(this WebApplication app)
    {
        // Unhandled exceptions are logged and returned as a generic 500 without internal details.
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        return app;
    }

    private static string ErrorCodeFor(int? statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => ErrorCodes.BadRequest,
        StatusCodes.Status401Unauthorized => ErrorCodes.Unauthorized,
        StatusCodes.Status403Forbidden => ErrorCodes.Forbidden,
        StatusCodes.Status404NotFound => ErrorCodes.NotFound,
        StatusCodes.Status405MethodNotAllowed => ErrorCodes.MethodNotAllowed,
        StatusCodes.Status409Conflict => ErrorCodes.Conflict,
        StatusCodes.Status415UnsupportedMediaType => ErrorCodes.UnsupportedMediaType,
        StatusCodes.Status429TooManyRequests => ErrorCodes.TooManyRequests,
        >= StatusCodes.Status500InternalServerError => ErrorCodes.InternalError,
        _ => ErrorCodes.Unknown,
    };
}
