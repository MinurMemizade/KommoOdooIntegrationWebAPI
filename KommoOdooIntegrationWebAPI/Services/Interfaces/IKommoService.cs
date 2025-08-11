using KommoOdooIntegrationWebAPI.Models.DTOs;

namespace KommoOdooIntegrationWebAPI.Services.Interfaces
{
    public interface IKommoService
    {
        Task<string> CreateLeadAsync(LeadDTO leadDTO);
        Task<string> CreateContactAsync(ContactDTO contactDTO);
        Task<string> GetLeadsAsync();
        Task<string> GetContactsAsync();
        Task<string> GetContactByIdAsync(long contactId);
    }

}
