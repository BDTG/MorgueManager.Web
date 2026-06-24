using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MorgueManager.Web.Models;
using Supabase;

namespace MorgueManager.Web.Services;

public class ContactService : IContactService
{
    private readonly Client _supabase;
    private readonly IJSRuntime _js;

    public ContactService(Client supabase, IJSRuntime js)
    {
        _supabase = supabase;
        _js = js;
    }

    public async Task<bool> SubmitContactAsync(ContactModel model)
    {
        try
        {
            model.Id = 0; // Let DB handle auto-increment
            model.CreatedAt = DateTime.UtcNow;
            if (string.IsNullOrEmpty(model.Status))
            {
                model.Status = "Chưa xử lý";
            }

            await _supabase.From<ContactModel>().Insert(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error submitting contact request to Supabase: {ex.Message}");
        }

        // Always save to LocalStorage cache/fallback to ensure it immediately shows in the local Admin dashboard
        try
        {
            var localDataJson = await _js.InvokeAsync<string>("localStorage.getItem", "local_contact_requests");
            var localRequests = string.IsNullOrEmpty(localDataJson) 
                ? new List<ContactModel>() 
                : JsonSerializer.Deserialize<List<ContactModel>>(localDataJson) ?? new List<ContactModel>();
            
            // Assign a local ID (>= 1000)
            model.Id = localRequests.Count > 0 ? localRequests.Max(r => r.Id) + 1 : 1000;
            localRequests.Add(model);
            
            await _js.InvokeVoidAsync("localStorage.setItem", "local_contact_requests", JsonSerializer.Serialize(localRequests));
            Console.WriteLine("Saved request to LocalStorage fallback/cache.");
        }
        catch (Exception jsex)
        {
            Console.WriteLine("Failed to save fallback to LocalStorage: " + jsex.Message);
        }

        return true;
    }
}
