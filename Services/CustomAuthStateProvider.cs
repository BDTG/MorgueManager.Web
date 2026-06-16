using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace MorgueManager.Web.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthService _authService;

    public CustomAuthStateProvider(AuthService authService)
    {
        _authService = authService;
        _authService.OnAuthStateChanged += HandleAuthStateChanged;
    }

    private void HandleAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var isLoggedIn = await _authService.IsLoggedInAsync();
        if (!isLoggedIn)
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(anonymous);
        }

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "Admin")
        }, "CustomAuth");

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public void Dispose()
    {
        _authService.OnAuthStateChanged -= HandleAuthStateChanged;
    }
}
