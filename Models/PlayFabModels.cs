namespace FirewallFunctionApp.Models;

/// <summary>
/// PlayFab CloudScript Azure Functions context models.
/// Based on https://github.com/PlayFab/PlayFab-Samples/blob/master/Samples/CSharp/AzureFunctions/CS2AFHelperClasses.cs
/// </summary>
public class TitleAuthenticationContext
{
    public string? Id { get; set; }
    public string? EntityToken { get; set; }
}

public class FunctionExecutionContext<T>
{
    public PlayFab.ProfilesModels.EntityProfileBody? CallerEntityProfile { get; set; }
    public TitleAuthenticationContext? TitleAuthenticationContext { get; set; }
    public bool? GeneratePlayStreamEvent { get; set; }
    public T? FunctionArgument { get; set; }
}

public class FunctionExecutionContext : FunctionExecutionContext<object>
{
}
