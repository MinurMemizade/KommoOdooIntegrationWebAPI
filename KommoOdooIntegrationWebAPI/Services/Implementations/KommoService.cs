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
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6IjViZmYxNWZhOWM0MThlYjQ1YjVmODNlNThiMmE5YTNlOGVjMzdhYzNmODVlYjNmODdkNzZmMjUxMWIwMjgyNjhlODZjMjY4ZTdhODJiZTRjIn0.eyJhdWQiOiJlODg2ZGRlOS02NjJjLTRjM2QtOTBjNy04Y2VlNDM4N2I0NjUiLCJqdGkiOiI1YmZmMTVmYTljNDE4ZWI0NWI1ZjgzZTU4YjJhOWEzZThlYzM3YWMzZjg1ZWIzZjg3ZDc2ZjI1MTFiMDI4MjY4ZTg2YzI2OGU3YTgyYmU0YyIsImlhdCI6MTc1NDY1MjQ5OCwibmJmIjoxNzU0NjUyNDk4LCJleHAiOjE3NTY2ODQ4MDAsInN1YiI6IjEzNjQ0MDMyIiwiZ3JhbnRfdHlwZSI6IiIsImFjY291bnRfaWQiOjM0OTk3MTU2LCJiYXNlX2RvbWFpbiI6ImtvbW1vLmNvbSIsInZlcnNpb24iOjIsInNjb3BlcyI6WyJjcm0iLCJmaWxlcyIsImZpbGVzX2RlbGV0ZSIsIm5vdGlmaWNhdGlvbnMiLCJwdXNoX25vdGlmaWNhdGlvbnMiXSwidXNlcl9mbGFncyI6MCwiaGFzaF91dWlkIjoiMTVmY2Y4ZWUtZWZmZC00MmNiLTlhNDgtNzFjMzE1YmE1ODFlIiwiYXBpX2RvbWFpbiI6ImFwaS1jLmtvbW1vLmNvbSJ9.b-2C4-AamkEW08N7gE_73kkl5f_PqXYVG0zlzni2YEPr8I-Foaqcczcere72asR95Hj2Cq3iGHVKEbkuMu-YdWcAs1o4DFd20APfBfcQSSO91_dUTe11DqfEa6a-9m2HbcfRkvq2RhlhGoR7cK8d5M6wiLXpnWwx0D4rYgNLZDBLrk8fjcaPZW6ejNaiowrdWzfvau1N8A-1wJwdy2QYSNAxufLiYNViUPdQTGS8oRnweuByJMnaFQ28veo8ve1CsdsXne08pguJ20CWG86Vy0kN2LMuM7D_XqrRPhB1B0-qGVioB0P881rTOLW2qP2gOxm7jbIPE5BkxQ2d-Tg43A");
        }

        public async Task<string> CreateLeadAsync(LeadDTO leadDTO)
        {
            var json = JsonSerializer.Serialize(new[] {leadDTO});
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("leads", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetLeadsAsync()
        {
            var response = await _httpClient.GetAsync("leads");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response content:");
            Console.WriteLine(content);

            return content;
        }
    }
}
