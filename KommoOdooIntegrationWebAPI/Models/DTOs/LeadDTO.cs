namespace KommoOdooIntegrationWebAPI.Models.DTOs
{
    public class LeadDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int StatusId { get; set; }
        public int PipelineId { get; set; }
        public int ResponsibleUserId { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public List<CustomFieldValue> CustomFieldsValues { get; set; }
        public List<ContactDto> Contacts { get; set; }
    }

    public class CustomFieldValue
    {
        public int FieldId { get; set; }
        public List<CustomFieldItem> Values { get; set; }
    }

    public class CustomFieldItem
    {
        public string Value { get; set; }
    }

    public class ContactDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

}
