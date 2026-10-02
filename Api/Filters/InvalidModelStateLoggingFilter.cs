using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipInvalidModelStateLoggingAttribute : Attribute, IFilterMetadata;

public sealed class InvalidModelStateLoggingFilter(ILogger<InvalidModelStateLoggingFilter> logger) : IAsyncResourceFilter, IAsyncActionFilter, IOrderedFilter
{
    public int Order => int.MinValue;

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        if (!ShouldSkip(context.Filters))
        {
            context.HttpContext.Request.EnableBuffering();
        }

        await next();
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (ShouldSkip(context.Filters))
        {
            await next();
            return;
        }

        var loggedInvalidModelState = false;
        if (!context.ModelState.IsValid)
        {
            await LogInvalidModelStateAsync(context);
            loggedInvalidModelState = true;
        }

        await next();

        if (!loggedInvalidModelState && !context.ModelState.IsValid)
        {
            await LogInvalidModelStateAsync(context);
        }
    }

    private static bool ShouldSkip(IList<IFilterMetadata> filters) =>
        filters.Any(filter => filter is SkipInvalidModelStateLoggingAttribute);

    private async Task LogInvalidModelStateAsync(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        var validationErrors = string.Join(
            "; ",
            context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}")));

        logger.LogWarning(
            "Invalid ModelState for {Method} {Path}. Request body: {RequestBody}. Validation errors: {ValidationErrors}",
            request.Method,
            request.Path,
            body,
            validationErrors);
    }
}
