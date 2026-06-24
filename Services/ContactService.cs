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

            // Run Supabase Insert with 2-second timeout
            var insertTask = _supabase.From<ContactModel>().Insert(model);
            var delayTask = Task.Delay(2000);
            var completedTask = await Task.WhenAny(insertTask, delayTask);

            if (completedTask == insertTask)
            {
                var response = await insertTask;
                // If Supabase returns the inserted record with generated ID, use it
                if (response?.Models?.Count > 0)
                {
                    Console.WriteLine($"Inserted to Supabase with Id={response.Models[0].Id}");
                }
            }
            else
            {
                Console.WriteLine("Supabase insert timed out after 2 seconds. Falling back to LocalStorage.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error submitting contact request to Supabase: {ex.Message}");
        }

        // Always save to LocalStorage cache/fallback to ensure it immediately shows in the local Admin dashboard
        try
        {
            var localDataJson = await _js.InvokeAsync<string>("localStorage.getItem", "local_contact_requests");
            List<ContactModelDto> localRequests = new List<ContactModelDto>();
            if (!string.IsNullOrEmpty(localDataJson))
            {
                try
                {
                    localRequests = JsonSerializer.Deserialize<List<ContactModelDto>>(localDataJson) ?? new List<ContactModelDto>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Corrupt JSON in local_contact_requests. Wiping it out: " + ex.Message);
                    localRequests = new List<ContactModelDto>();
                }
            }
            
            var dto = ContactModelDto.FromModel(model);
            // Assign a local ID (>= 1000) to differentiate from Supabase DB IDs
            dto.Id = localRequests.Count > 0 ? localRequests.Max(r => r.Id) + 1 : 1000;
            model.Id = dto.Id; // Sync back to the source model
            
            localRequests.Add(dto);
            
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
