using System.Text.Json;
using KommoOdooIntegrationWebAPI.Models.DTOs;
using KommoOdooIntegrationWebAPI.Services.Interfaces;

namespace KommoOdooIntegrationWebAPI.Services.Implementations
{
    public class SyncService : ISyncService
    {
        private readonly IKommoService _kommoService;
        private readonly IOdooService _odooService;

        public SyncService(IKommoService kommoService, IOdooService odooService)
        {
            _kommoService = kommoService;
            _odooService = odooService;
        }

        public async Task KommoToOdooIntegrationAsync()
        {
            var json = await _kommoService.GetLeadsAsync("contacts,companies");
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("_embedded", out var embedded) ||
                !embedded.TryGetProperty("leads", out var leadsElement))
            {
                Console.WriteLine("No leads found in response.");
                return;
            }

            foreach (var lead in leadsElement.EnumerateArray())
            {
                string leadName = lead.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "No name";
                decimal? revenue = lead.TryGetProperty("price", out var priceProp) && priceProp.ValueKind == JsonValueKind.Number
                                   ? priceProp.GetDecimal()
                                   : (decimal?)null;
                string description = $"View full lead in Kommo: https://memizademinur.kommo.com/leads/detail/{lead.GetProperty("id").GetInt64()}";

                string companyName = null;
                string contactName = null;
                string contactEmail = null;
                string contactPhone = null;

                if (lead.TryGetProperty("_embedded", out var leadEmbedded) &&
                    leadEmbedded.TryGetProperty("contacts", out var contactsArray) &&
                    contactsArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var contactRef in contactsArray.EnumerateArray())
                    {
                        if (contactRef.TryGetProperty("id", out var contactIdProp))
                        {
                            long contactId = contactIdProp.GetInt64();

                            var contactJson = await _kommoService.GetContactByIdAsync(contactId);
                            using var contactDoc = JsonDocument.Parse(contactJson);
                            var contactRoot = contactDoc.RootElement;

                            contactName = contactRoot.TryGetProperty("name", out var cName) ? cName.GetString() : null;

                            if (contactRoot.TryGetProperty("custom_fields_values", out var cfv) &&
                                cfv.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var cf in cfv.EnumerateArray())
                                {
                                    string fieldCode = cf.TryGetProperty("field_code", out var fc) ? fc.GetString() : null;
                                    string val = cf.TryGetProperty("values", out var values) &&
                                                 values.ValueKind == JsonValueKind.Array &&
                                                 values.GetArrayLength() > 0
                                                 ? values[0].GetProperty("value").GetString()
                                                 : null;

                                    if (fieldCode == "EMAIL" && !string.IsNullOrWhiteSpace(val))
                                        contactEmail = val;
                                    if (fieldCode == "PHONE" && !string.IsNullOrWhiteSpace(val))
                                        contactPhone = val;
                                }
                            }
                        }
                    }
                }

                if (leadEmbedded.TryGetProperty("companies", out var companiesArray) &&
                    companiesArray.ValueKind == JsonValueKind.Array &&
                    companiesArray.GetArrayLength() > 0)
                {
                    var company = companiesArray[0];
                    companyName = company.TryGetProperty("name", out var cName) ? cName.GetString() : null;

                    if (string.IsNullOrWhiteSpace(contactEmail))
                        contactEmail = company.TryGetProperty("email", out var cEmail) ? cEmail.GetString() : null;
                    if (string.IsNullOrWhiteSpace(contactPhone))
                        contactPhone = company.TryGetProperty("phone", out var cPhone) ? cPhone.GetString() : null;
                }

                var contactIdOdoo = await _odooService.CreateAsync("res.partner", new
                {
                    name = contactName ?? leadName,
                    email = contactEmail,
                    phone = contactPhone,
                    company_name = companyName
                });
                int contactIdInt = Convert.ToInt32(contactIdOdoo);

                await _odooService.CreateAsync("crm.lead", new
                {
                    name = leadName,
                    partner_id = contactIdInt,
                    expected_revenue = revenue,
                    description = description
                });
            }
        }


        public async Task OdooToKommoIntegrationAsync()
        {
            int urlFieldId = 1457056;

            var existingLeadsJson = await _kommoService.GetAsync("/api/v4/leads");
            var existingLeadLinks = new HashSet<string>();

            if (existingLeadsJson == null)
            {
                using (var doc = JsonDocument.Parse(existingLeadsJson))
                {
                    if (doc.RootElement.TryGetProperty("_embedded", out var embedded) &&
                        embedded.TryGetProperty("leads", out var leadsArray))
                    {
                        foreach (var lead in leadsArray.EnumerateArray())
                        {
                            if (lead.TryGetProperty("custom_fields_values", out var customFields))
                            {
                                foreach (var field in customFields.EnumerateArray())
                                {
                                    if (field.TryGetProperty("field_id", out var fieldIdProp) && fieldIdProp.GetInt32() == urlFieldId &&
                                        field.TryGetProperty("values", out var valuesArray))
                                    {
                                        foreach (var val in valuesArray.EnumerateArray())
                                        {
                                            if (val.TryGetProperty("value", out var linkProp))
                                                existingLeadLinks.Add(linkProp.GetString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var fields = new string[] { "id", "name" };
            var jsonResult = await _odooService.SearchReadAsync("crm.lead", new object[][] { }, fields);

            int createdCount = 0;

            foreach (var item in jsonResult.GetProperty("result").EnumerateArray())
            {
                var leadId = item.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.Number
                    ? idProp.GetInt32()
                    : 0;
                if (leadId == 0) continue;

                string odooLink = $"https://pixelzone.odoo.com/web#id={leadId}&model=crm.lead";

                if (existingLeadLinks.Contains(odooLink)) continue;

                string leadName = item.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String
                    ? nameProp.GetString()
                    : "NoName";

                var leadPayload = new[]
                {
                    new
                    {
                        name = leadName,
                        custom_fields_values = new[]
                        {
                            new
                            {
                                field_id = urlFieldId,
                                values = new[] { new { value = odooLink } }
                            }
                        }
                    }
                };

                try
                {
                    var leadJsonString = await _kommoService.PostAsync("/api/v4/leads", leadPayload);
                    var leadJson = JsonDocument.Parse(leadJsonString);
                    if (leadJson.RootElement.TryGetProperty("_embedded", out var embeddedResp) &&
                        embeddedResp.TryGetProperty("leads", out var leadsRespArray) &&
                        leadsRespArray.GetArrayLength() > 0 &&
                        leadsRespArray[0].TryGetProperty("id", out var createdLeadId))
                    {
                        createdCount++;
                        Console.WriteLine($"Lead yaradıldı: {createdLeadId.GetInt32()} (Odoo link: {odooLink})");
                    }
                    else
                    {
                        Console.WriteLine($"Lead yaradıla bilmədi: {leadJsonString}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Xəta lead üçün {odooLink}: {ex.Message}");
                }
            }

            Console.WriteLine($"Bütün Odoo leadləri Kommo-da yaradıldı. Uğurla yaradılan lead sayı: {createdCount}");
        }




    }

}


