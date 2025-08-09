using KommoOdooIntegrationWebAPI.Models.DTOs;

namespace KommoOdooIntegrationWebAPI.Services.Interfaces
{
    public interface IKommoService
    {
        Task<string> GetLeadsAsync();
        Task<string> CreateLeadAsync(LeadDTO leadDTO);
    }
}
