using KommoOdooIntegrationWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KommoOdooIntegrationWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly ISyncService _syncService;

        public IntegrationController(ISyncService syncService)
        {
            _syncService = syncService;
        }


        [HttpPost("sync/odoo-to-kommo")]
        public async Task<IActionResult> SyncOdooToKommo()
        {
            try
            {
                await _syncService.OdooToKommoIntegrationAsync();
                return Ok(new { message = "Odoo to Kommo sinxronizasiyası uğurla başa çatdı." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("sync/kommo-to-odoo")]
        public async Task<IActionResult> SyncKommoToOdoo()
        {
            try
            {
                await _syncService.KommoToOdooIntegrationAsync();
                return Ok(new { message = "Kommo to Odoo sinxronizasiyası uğurla başa çatdı." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
