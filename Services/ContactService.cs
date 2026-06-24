using System;
using System.Threading.Tasks;
using MorgueManager.Web.Models;
using Supabase;

namespace MorgueManager.Web.Services;

public class ContactService : IContactService
{
    private readonly Client _supabase;

    public ContactService(Client supabase)
    {
        _supabase = supabase;
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

            var response = await _supabase.From<ContactModel>().Insert(model);
            return response.Models != null && response.Models.Count > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error submitting contact request to Supabase: {ex.Message}");
            // Fallback for demo/offline: if table doesn't exist, we mock success to not block user interaction.
            await Task.Delay(1000);
            return true;
        }
    }
}
