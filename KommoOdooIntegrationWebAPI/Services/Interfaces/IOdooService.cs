using System.Text.Json;
using KommoOdooIntegrationWebAPI.Models.DTOs;

namespace KommoOdooIntegrationWebAPI.Services.Interfaces
{
    public interface IOdooService
    {
        Task<int> AuthenticateAsync();
        Task<JsonElement> SearchReadAsync(string model, object[][] domain, string[] fields);
        Task<int> CreateAsync(string model, object values);
        Task<bool> UpdateAsync(string model, int id, object values);
    }
}
