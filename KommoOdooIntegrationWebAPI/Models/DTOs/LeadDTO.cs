namespace KommoOdooIntegrationWebAPI.Models.DTOs
{
    public class LeadDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CustomFieldValue[] CustomFieldsValues { get; set; }
        public EmbeddedContacts _embedded { get; set; }
    }

    public class CustomFieldValue
    {
        public string Field_code { get; set; }
        public Value[] Values { get; set; }
    }

    public class CustomFieldItem
    {
        public Value Value { get; set; }
    }

    public class ContactDTO
    {
        public string Name { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public CustomFieldValue[] Custom_fields_values { get; set; }
    }

    public class EmbeddedContacts
    {
        public ContactDTO[] Contacts { get; set; }
    }

    public class Value
    {
        public string value { get; set; }
    }

    public class OdooLeadDTO
    {
        public string model { get; set; }
        public object[][] domain { get; set; }
        public string[] fields { get; set; }
    }

    public class OdooLead
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Company { get; set; }
        public decimal Price { get; set; }
    }


}
