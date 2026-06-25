using System;
using System.Threading.Tasks;
using MorgueManager.Web.Models;

namespace MorgueManager.Web.Services;

public interface IContactService
{
    Task<bool> SubmitContactAsync(ContactModel model);
    Task<int> GetUnprocessedCountAsync();
    event Action? OnContactRequestsChanged;
    void NotifyRequestsChanged();
}

