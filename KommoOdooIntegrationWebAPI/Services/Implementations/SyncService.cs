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
            var json = await _kommoService.GetLeadsAsync();

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("_embedded", out var embedded) ||
                !embedded.TryGetProperty("leads", out var leadsElement))
            {
                Console.WriteLine("No leads found in response.");
                return;
            }

            foreach (var lead in leadsElement.EnumerateArray())
            {
                string name = lead.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "No name";

                string email = null;
                string phone = null;

                if (lead.TryGetProperty("_embedded", out var leadEmbedded) &&
                    leadEmbedded.TryGetProperty("contacts", out var contacts) &&
                    contacts.ValueKind == JsonValueKind.Array &&
                    contacts.GetArrayLength() > 0)
                {
                    var firstContact = contacts[0];

                    if (firstContact.TryGetProperty("custom_fields_values", out var cfv) &&
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

                            if (fieldCode == "EMAIL") email = val;
                            else if (fieldCode == "PHONE") phone = val;
                        }
                    }
                }

                var contactId = await _odooService.CreateAsync("res.partner", new
                {
                    name = name,
                    email = email,
                    phone = phone
                });

                int contactIdInt = Convert.ToInt32(contactId);

                await _odooService.CreateAsync("crm.lead", new
                {
                    name = name,
                    partner_id =  contactIdInt
                });
            }
        }


        public async Task OdooToKommoIntegrationAsync()
        {
            var odooLeads = await _odooService.SearchReadAsync(
                "res.partner",
                new object[][] { },
                new string[] { "name", "email", "phone" }
            );

            Console.WriteLine(odooLeads.ToString());

            var leads = odooLeads.GetProperty("result").EnumerateArray();
            foreach (var lead in leads)
            {
                string name = lead.GetProperty("name").GetString();
                string email = null;
                if (lead.TryGetProperty("email_from", out var em))
                {
                    if (em.ValueKind == JsonValueKind.String)
                        email = em.GetString();
                }

                string phone = null;
                if (lead.TryGetProperty("phone", out var ph))
                {
                    if (ph.ValueKind == JsonValueKind.String)
                        phone = ph.GetString();
                }

                var contactDto = new ContactDTO
                {
                    Name = name,
                    First_name = name?.Split(' ')[0] ?? name,
                    Last_name = name?.Contains(' ') == true ? name.Split(' ')[1] : "",
                    Custom_fields_values = new[]
                    {
                new CustomFieldValue
                {
                    Field_code = "EMAIL",
                    Values = new[] { new Value { value = email } }
                },
                new CustomFieldValue
                {
                    Field_code = "PHONE",
                    Values = new[] { new Value { value = phone } }
                }
            }
                };

                var leadDto = new LeadDTO
                {
                    Name = name,
                    _embedded = new EmbeddedContacts
                    {
                        Contacts = new[] { contactDto }
                    }
                };

                await _kommoService.CreateLeadAsync(leadDto);
            }
        }
    }
}
