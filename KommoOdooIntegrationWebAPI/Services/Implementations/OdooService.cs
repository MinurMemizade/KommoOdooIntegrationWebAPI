using System.Text;
using System.Text.Json;
using KommoOdooIntegrationWebAPI.Configurations;
using KommoOdooIntegrationWebAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace KommoOdooIntegrationWebAPI.Services.Implementations
{
    public class OdooService : IOdooService
    {
        private readonly HttpClient _httpClient;
        private readonly OdooConfiguration _odooConfiguration;
        private int _uid;
        public OdooService(HttpClient httpClient, IOptions<OdooConfiguration> odooConfiguration)
        {
            _httpClient = httpClient;
            _odooConfiguration = odooConfiguration.Value;
        }

        public async Task<int> AuthenticateAsync()
        {
            var request = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    service = "common",
                    method = "login",
                    args = new object[] { _odooConfiguration.Db, _odooConfiguration.Username, _odooConfiguration.Password }
                },
                id = 1
            };

            var response = await SendRequestAsync(request);
            _uid = response.GetProperty("result").GetInt32();
            return _uid;
        }

        public async Task<JsonElement> SearchReadAsync(string model, object domain, object fields)
        {
            var uid = await AuthenticateAsync();
            var request = new
            {
                jsonrpc = "2.0",
                method = "call",
                @params = new
                {
                    service = "object",
                    method = "execute_kw",
                    args = new object[]
                {
                    _odooConfiguration.Db, _uid=uid, _odooConfiguration.Password,
                    model, "search_read",
                    domain,
                    new { fields }
                }
                },
                id = 2
            };
            return await SendRequestAsync(request);
        }

        private async Task<JsonElement> SendRequestAsync(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://pixelzone.odoo.com/jsonrpc", content);
            response.EnsureSuccessStatusCode();
            
            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(body);
        }
    }
}
