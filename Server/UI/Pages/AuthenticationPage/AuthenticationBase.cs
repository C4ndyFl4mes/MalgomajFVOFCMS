using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Server.API.Routes.User.SignIn;
using Server.UI.Services;

namespace Server.UI.Pages.AuthenticationPage;

public class AuthenticationBase : ComponentBase, IAsyncDisposable
{
    [Inject]
    protected IJSRuntime JS { get; set; } = default!;
    [Inject]
    protected IValidator<SignInRequest> SignInValidator { get; set; } = default!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;
    
    protected SignInRequest SignInFields { get; set; } = new()
    {
        Email = string.Empty,
        Password = string.Empty
    };
    protected Dictionary<string, string[]> ValidationErrors { get; set; } = [];
    protected (bool IsSuccess, string Message) ResultMessage { get; set; } = (false, string.Empty);

    private IJSObjectReference? _authModule;


    protected async Task HandleSignInAsync()
    {
        ValidationErrors.Clear();
        ValidationResult validationResult = SignInValidator.Validate(SignInFields);

        if (!validationResult.IsValid)
        {
            ErrorsToDictionary(validationResult);
            await InvokeAsync(StateHasChanged);
            return;
        }

        APIResult<SignInResponse> result = await SignInFromBrowserAsync(SignInFields);

        if (!result.Success)
        {
            ResultMessage = (false, result.Error?.Detail ?? "Ett oväntat fel inträffade. Vänligen försök igen senare.");
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (result.Data is null)
        {
            ResultMessage = (false, "Ett oväntat fel inträffade. Vänligen försök igen senare.");
            await InvokeAsync(StateHasChanged);
            return;
        }

        ResultMessage = (true, result.Data.Message);
        await InvokeAsync(StateHasChanged);

        await Task.Delay(2000);
        NavigationManager.NavigateTo("/admin/panel", forceLoad: true);
    }

    private async Task<APIResult<SignInResponse>> SignInFromBrowserAsync(SignInRequest request)
    {
        _authModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "/fetch/authentication.js");
        return await _authModule.InvokeAsync<APIResult<SignInResponse>>("signin", request);
    }

    private void ErrorsToDictionary(ValidationResult validationResult)
    {
        ValidationErrors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }

    public async ValueTask DisposeAsync()
    {
        if (_authModule is not null)
        {
            await _authModule.DisposeAsync();
        }
    }
}