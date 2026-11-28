using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FieldOps.Api.Http;

/// <summary>
/// Runs the registered FluentValidation validator for each action argument before the action executes.
/// Failures are returned as a 400 ValidationProblemDetails response, in the same shape as model binding errors.
/// </summary>
internal sealed class ValidationFilter(ProblemDetailsFactory problemDetailsFactory) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            foreach (var failure in result.Errors)
            {
                context.ModelState.AddModelError(ToJsonPath(failure.PropertyName), failure.ErrorMessage);
            }
        }

        if (!context.ModelState.IsValid)
        {
            var problem = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);
            context.Result = new ObjectResult(problem) { StatusCode = problem.Status };
            return;
        }

        await next();
    }

    // Error keys must match the JSON property names the client sent (e.g. "Address.City" -> "address.city").
    private static string ToJsonPath(string propertyName) =>
        string.Join('.', propertyName.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
}
