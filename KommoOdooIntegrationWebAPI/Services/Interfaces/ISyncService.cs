namespace KommoOdooIntegrationWebAPI.Services.Interfaces
{
    public interface ISyncService
    {
        Task OdooToKommoIntegrationAsync();
        Task KommoToOdooIntegrationAsync();
    }
}
