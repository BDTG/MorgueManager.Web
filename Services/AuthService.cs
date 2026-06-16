using Microsoft.JSInterop;

namespace MorgueManager.Web.Services;

public class AuthService
{
    private readonly IJSRuntime _js;
    private static string? _memoryToken;

    public event Action? OnAuthStateChanged;

    public AuthService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> IsLoggedInAsync()
    {
        if (!string.IsNullOrEmpty(_memoryToken))
            return true;

        try
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "morguemanager-token");
            if (!string.IsNullOrEmpty(token))
            {
                _memoryToken = token;
                return true;
            }
        }
        catch { }

        return false;
    }

    public async Task LoginAsync(string token)
    {
        _memoryToken = token;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "morguemanager-token", token);
        }
        catch { }
        OnAuthStateChanged?.Invoke();
    }

    public async Task LogoutAsync()
    {
        _memoryToken = null;
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "morguemanager-token");
        }
        catch { }
        OnAuthStateChanged?.Invoke();
    }

    public async Task<string?> LoginWithGoogleAsync()
    {
        try
        {
            // Gọi hàm Javascript 'window.loginWithGoogle'
            var result = await _js.InvokeAsync<GoogleLoginResult>("loginWithGoogle");
            if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
            {
                await LoginAsync(result.Token);
                return null; // Thành công, không có lỗi
            }
            
            return result?.ErrorMessage ?? "Lỗi không xác định từ Google Sign-in";
        }
        catch (Exception ex)
        {
            return "Lỗi kết nối Firebase: " + ex.Message;
        }
    }
}

public class GoogleLoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
