using System.Text;
using System.Text.Json;
using KommoOdooIntegrationWebAPI.Configurations;
using KommoOdooIntegrationWebAPI.Models.DTOs;
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
                    method = "authenticate",
                    args = new object[]
                    {
                        _odooConfiguration.Db,
                        _odooConfiguration.Username,
                        _odooConfiguration.Password,
                        new { }
                    }
                },
                id = 1
            };

            var response = await SendRequestAsync(request);
            _uid = response.GetProperty("result").GetInt32();
            return _uid;
        }

        public async Task<JsonElement> SearchReadAsync(string model, object[][] domain, string[] fields)
        {
            if (_uid == 0)
                await AuthenticateAsync();

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
                        _odooConfiguration.Db,
                        _uid,
                        _odooConfiguration.Password,
                        model,
                        "search_read",
                        new object[] { domain },
                        new { fields }
                    }
                },
                id = 2
            };

            return await SendRequestAsync(request);
        }

        public async Task<int> CreateAsync(string model, object values)
        {
            if (_uid == 0)
                await AuthenticateAsync();

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
                        _odooConfiguration.Db,
                        _uid,
                        _odooConfiguration.Password,
                        model,
                        "create",
                        new object[] { values }
                    }
                },
                id = 3
            };

            var response = await SendRequestAsync(request);
            return response.GetProperty("result").GetInt32();
        }

        public async Task<bool> UpdateAsync(string model, int id, object values)
        {
            if (_uid == 0)
                await AuthenticateAsync();

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
                    _odooConfiguration.Db,
                    _uid,
                    _odooConfiguration.Password,
                    model,
                    "write",
                    new object[] { new int[] { id }, values }
                    }
                },
                id = 4
            };

            var response = await SendRequestAsync(request);
            return response.GetProperty("result").GetBoolean();
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
