using System.Threading.Tasks;
using MorgueManager.Web.Models;

namespace MorgueManager.Web.Services;

public class ContactService : IContactService
{
    private readonly ApiService _apiService;

    public ContactService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> SubmitContactAsync(ContactModel model)
    {
        // Mock a network request delay of 1.2 seconds, simulating a call to the backend.
        await Task.Delay(1200);
        return true;
    }
}
