using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AIAPI.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeader = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedKey = config["ApiKey"];

        if (string.IsNullOrEmpty(expectedKey))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey)
            || providedKey != expectedKey)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }
}
