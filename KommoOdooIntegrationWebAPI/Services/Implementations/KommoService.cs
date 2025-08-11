using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KommoOdooIntegrationWebAPI.Models.DTOs;
using KommoOdooIntegrationWebAPI.Services.Interfaces;

namespace KommoOdooIntegrationWebAPI.Services.Implementations
{
    public class KommoService : IKommoService
    {
        private readonly HttpClient _httpClient;

        public KommoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://memizademinur.kommo.com/api/v4/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjA4YzU1ODhhMjNkM2NkZjA1M2I4ZmU2NzUyMzFlYjdhNjU3NzliNGE3NTEyNTk5NDM5MmQxODYxYmMxMDJjNTVjYzM0NDJlMjg2YzM5Y2M1In0.eyJhdWQiOiJlODg2ZGRlOS02NjJjLTRjM2QtOTBjNy04Y2VlNDM4N2I0NjUiLCJqdGkiOiIwOGM1NTg4YTIzZDNjZGYwNTNiOGZlNjc1MjMxZWI3YTY1Nzc5YjRhNzUxMjU5OTQzOTJkMTg2MWJjMTAyYzU1Y2MzNDQyZTI4NmMzOWNjNSIsImlhdCI6MTc1NDgyNDA0NiwibmJmIjoxNzU0ODI0MDQ2LCJleHAiOjE3NTY2ODQ4MDAsInN1YiI6IjEzNjQ0MDMyIiwiZ3JhbnRfdHlwZSI6IiIsImFjY291bnRfaWQiOjM0OTk3MTU2LCJiYXNlX2RvbWFpbiI6ImtvbW1vLmNvbSIsInZlcnNpb24iOjIsInNjb3BlcyI6WyJjcm0iLCJmaWxlcyIsImZpbGVzX2RlbGV0ZSIsIm5vdGlmaWNhdGlvbnMiLCJwdXNoX25vdGlmaWNhdGlvbnMiXSwidXNlcl9mbGFncyI6MCwiaGFzaF91dWlkIjoiYzYxM2RkMjQtZDQ2Ni00NDdmLTk5MTYtMmVjMmExMmY2NGVlIiwiYXBpX2RvbWFpbiI6ImFwaS1jLmtvbW1vLmNvbSJ9.JfBysuxyMbBEvSYWOSUojuUlAysjer5Y5JeiiDBbq5K-ZWa8EMXigoCBnsf0ehU5VoAmkWte8AbdYSKP-3UTH6bXJqHkGQ9tu_TtJgZ82a0ObVGOl_LEXIPhqvlha1LpPpSBig_TXPCsI0nQyP17SiAACJHuh_v9Vaq5tces_NtamiNaDKyJfru4wMusUdlEi5yERFpLmLSIzhCuS_6bt1H3FbsXWLH6CyeSe7NVxzN7bpmZyGyXLB4sKmd5gazDiBqjJHqJgw-IHqJ3IfH_p6VoE28K06tFTodWsRi7t1k8zMun6gJoGihAPwhpdXODF8PIKQZX0a3qvkJKTwoD4w");
        }

        private async Task<string> PostAsync(string endpoint, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


        public Task<string> CreateLeadAsync(LeadDTO leadDTO)
            => PostAsync("leads", new[] { leadDTO });

        public Task<string> CreateContactAsync(ContactDTO contactDTO)
            => PostAsync("contacts", new[] { contactDTO });

        public Task<string> GetLeadsAsync()
            => GetAsync("leads");

        public Task<string> GetContactsAsync()
            => GetAsync("contacts");

        public Task<string> GetContactByIdAsync(long contactId)
            => GetAsync($"contacts/{contactId}");
    }
}
