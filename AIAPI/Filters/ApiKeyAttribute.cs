using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AIAPI.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ApiKeyAttribute(string configKey = "ApiKeys:Monitoring") : Attribute, IAuthorizationFilter
{
    private const string ApiKeyHeader = "X-Api-Key";
    private readonly string _configKey = configKey;

    // Authorization filter: draait vóór model-binding, zodat een ontbrekende of
    // verkeerde API-key altijd met 401 wordt geweigerd (ook bij POST zonder body).
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedKey = config[_configKey];

        if (string.IsNullOrEmpty(expectedKey))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey)
            || providedKey != expectedKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
