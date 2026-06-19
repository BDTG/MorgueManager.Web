using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MorgueManager.Web.Pages.Admin;

public static class AdminAuthHelper
{
    public static string DecodeJwtPayload(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2) return "";
            var payload = parts[1];
            string base64 = payload.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }

    public static Dictionary<string, object>? ParseClaims(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;
        var json = DecodeJwtPayload(token);
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch
        {
            return null;
        }
    }

    public static string GetRoleFromToken(string token)
    {
        var claims = ParseClaims(token);
        if (claims == null) return "None";

        if (claims.TryGetValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out var roleObj))
        {
            return roleObj?.ToString() ?? "None";
        }
        if (claims.TryGetValue("role", out roleObj))
        {
            return roleObj?.ToString() ?? "None";
        }
        return "None";
    }

    public static string GetEmailFromToken(string token)
    {
        var claims = ParseClaims(token);
        if (claims == null) return "";

        if (claims.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out var emailObj))
        {
            return emailObj?.ToString() ?? "";
        }
        if (claims.TryGetValue("email", out emailObj))
        {
            return emailObj?.ToString() ?? "";
        }
        if (claims.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out var nameObj))
        {
            return nameObj?.ToString() ?? "";
        }
        return "";
    }

    public static bool IsTokenExpired(string token)
    {
        var claims = ParseClaims(token);
        if (claims == null) return true;

        if (claims.TryGetValue("exp", out var expObj))
        {
            try
            {
                // The exp claim is numeric, which can be deserialized as JsonElement
                long expSeconds = 0;
                if (expObj is JsonElement element)
                {
                    expSeconds = element.GetInt64();
                }
                else
                {
                    expSeconds = Convert.ToInt64(expObj.ToString());
                }
                var expTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                return DateTime.UtcNow >= expTime;
            }
            catch
            {
                return true;
            }
        }
        return false;
    }

    public static async Task<string> GetUserRoleAsync(IJSRuntime js)
    {
        try
        {
            var token = await js.InvokeAsync<string>("localStorage.getItem", "morguemanager-token");
            if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
            {
                return "None";
            }
            return GetRoleFromToken(token);
        }
        catch
        {
            return "None";
        }
    }

    public static async Task<string> GetUserEmailAsync(IJSRuntime js)
    {
        try
        {
            var token = await js.InvokeAsync<string>("localStorage.getItem", "morguemanager-token");
            if (string.IsNullOrEmpty(token)) return "";
            return GetEmailFromToken(token);
        }
        catch
        {
            return "";
        }
    }
}
