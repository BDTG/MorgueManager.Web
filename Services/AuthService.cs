using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Supabase;
using Supabase.Gotrue;
using static Supabase.Gotrue.Constants;

namespace MorgueManager.Web.Services;

public class AuthService
{
    private readonly Supabase.Client _supabase;
    private readonly IJSRuntime _js;
    private readonly NavigationManager _navigationManager;

    public event Action? OnAuthStateChanged;

    public AuthService(Supabase.Client supabase, IJSRuntime js, NavigationManager navigationManager)
    {
        _supabase = supabase;
        _js = js;
        _navigationManager = navigationManager;

        try
        {
            _supabase.Auth.AddStateChangedListener((client, state) =>
            {
                OnAuthStateChanged?.Invoke();
            });
        }
        catch { }
    }

    public async Task<bool> IsLoggedInAsync()
    {
        if (_supabase.Auth.CurrentSession != null)
            return true;

        try
        {
            var currentUri = new Uri(_navigationManager.Uri);
            if (currentUri.Fragment.Contains("access_token=") || currentUri.Query.Contains("code="))
            {
                var session = await _supabase.Auth.GetSessionFromUrl(currentUri, true);
                if (session != null)
                {
                    if (session.AccessToken != null)
                    {
                        await _js.InvokeVoidAsync("localStorage.setItem", "morguemanager-token", session.AccessToken);
                    }
                    return true;
                }
            }

            var token = await _js.InvokeAsync<string>("localStorage.getItem", "morguemanager-token");
            if (!string.IsNullOrEmpty(token))
            {
                return true;
            }
        }
        catch { }

        return false;
    }

    public async Task LoginAsync(string token)
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "morguemanager-token", token);
        }
        catch { }
        OnAuthStateChanged?.Invoke();
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "morguemanager-token");
            await _supabase.Auth.SignOut();
        }
        catch { }
        OnAuthStateChanged?.Invoke();
    }

    public async Task<string?> LoginWithGoogleAsync()
    {
        try
        {
            var options = new SignInOptions { RedirectTo = _navigationManager.ToAbsoluteUri("/admin/dashboard").ToString() };
            var state = await _supabase.Auth.SignIn(Provider.Google, options);
            if (state?.Uri != null)
            {
                return state.Uri.ToString();
            }
            return null;
        }
        catch (Exception ex)
        {
            return "Lỗi kết nối Supabase Google: " + ex.Message;
        }
    }

    public async Task<bool> SendOtpAsync(string email)
    {
        try
        {
            var options = new SignInWithPasswordlessEmailOptions(email);
            var state = await _supabase.Auth.SignInWithOtp(options);
            return state != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> VerifyOtpAsync(string email, string token)
    {
        try
        {
            var session = await _supabase.Auth.VerifyOTP(email, token, EmailOtpType.MagicLink);
            if (session != null)
            {
                if (session.AccessToken != null)
                {
                    await LoginAsync(session.AccessToken);
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
