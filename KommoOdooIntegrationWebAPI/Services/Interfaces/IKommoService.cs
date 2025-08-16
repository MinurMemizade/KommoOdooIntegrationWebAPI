using KommoOdooIntegrationWebAPI.Models.DTOs;

namespace KommoOdooIntegrationWebAPI.Services.Interfaces
{
    public interface IKommoService
    {
        Task<string> CreateLeadAsync(LeadDTO leadDTO);
        Task<string> CreateContactAsync(ContactDTO contactDTO);
        Task<string> GetLeadsAsync(string with = null, int page = 1, int limit = 250);
        Task<string> GetContactsAsync();
        Task<string> GetContactByIdAsync(long contactId);
        Task<string> UpdateLeadAsync(long leadId, LeadDTO leadDTO);
        Task<string> UpdateContactAsync(long contactId, ContactDTO contactDTO);
        Task<string> PostAsync(string endpoint, object payload);
        Task<string> GetAsync(string endpoint);
    }
}
