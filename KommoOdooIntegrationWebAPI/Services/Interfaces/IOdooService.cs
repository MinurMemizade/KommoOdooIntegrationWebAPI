using System.Text.Json;

namespace KommoOdooIntegrationWebAPI.Services.Interfaces
{
    public interface IOdooService
    {
        Task<int> AuthenticateAsync();
        Task<JsonElement> SearchReadAsync(string model, object domain, object fields);
    }
}
