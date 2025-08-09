using KommoOdooIntegrationWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KommoOdooIntegrationWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OdooController : ControllerBase
    {
        private readonly IOdooService _odooService;

        public OdooController(IOdooService odooService)
        {
            _odooService = odooService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate()
        {
            try
            {
                await _odooService.AuthenticateAsync();
                return Ok(new { message = "Authentication successful" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("searchread")]
        public async Task<IActionResult> SearchRead([FromBody] SearchReadRequest request)
        {
            try
            {
                var result = await _odooService.SearchReadAsync(request.Model, request.Domain, request.Fields);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        public class SearchReadRequest
        {
            public string Model { get; set; } = string.Empty;
            public object Domain { get; set; } = new object[] { };
            public object Fields { get; set; } = new object[] { };
        }

    }
}
