using System.Text.Json;

namespace KommoOdooIntegrationWebAPI.Services.Interfaces
{
    public interface IOdooService
    {
        Task<int> AuthenticateAsync();
        Task<JsonElement> SearchReadAsync(string model, object[][] domain, string[] fields);
        Task<int> CreateAsync(string model, object values);
    }

}
