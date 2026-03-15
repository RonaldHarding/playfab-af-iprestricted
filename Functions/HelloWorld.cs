using System.Text.Json;
using FirewallFunctionApp.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FirewallFunctionApp.Functions;

public class HelloWorld
{
    private readonly ILogger<HelloWorld> _logger;

    public HelloWorld(ILogger<HelloWorld> logger)
    {
        _logger = logger;
    }

    [Function("HelloWorld")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var requestBody = await req.ReadAsStringAsync();

        _logger.LogInformation("HelloWorld function invoked. Request body: {RequestBody}", requestBody);

        FunctionExecutionContext<JsonElement>? context = null;
        try
        {
            context = JsonSerializer.Deserialize<FunctionExecutionContext<JsonElement>>(requestBody ?? "",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize PlayFab context from request body");
        }

        if (context?.CallerEntityProfile != null)
        {
            _logger.LogInformation("Caller Entity Id: {EntityId}, Type: {EntityType}",
                context.CallerEntityProfile.Entity?.Id,
                context.CallerEntityProfile.Entity?.Type);
        }

        if (context?.TitleAuthenticationContext != null)
        {
            _logger.LogInformation("Title Id: {TitleId}", context.TitleAuthenticationContext.Id);
        }

        // Validate the request comes from an allowed PlayFab title
        var allowedTitles = Environment.GetEnvironmentVariable("AllowedTitles")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        var titleId = context?.TitleAuthenticationContext?.Id;

        if (allowedTitles.Length > 0 && !allowedTitles.Contains(titleId, StringComparer.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Rejected request from unauthorized title: {TitleId}", titleId);
            var forbidden = req.CreateResponse(System.Net.HttpStatusCode.Forbidden);
            await forbidden.WriteAsJsonAsync(new { error = "Unauthorized title" });
            return forbidden;
        }

        if (context != null &&
            context.FunctionArgument.ValueKind != JsonValueKind.Undefined &&
            context.FunctionArgument.ValueKind != JsonValueKind.Null)
        {
            _logger.LogInformation("Function arguments: {Args}", context.FunctionArgument.GetRawText());
        }

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            messageValue = "Hello from FirewallFunctionApp!",
            success = true
        });

        return response;
    }
}
