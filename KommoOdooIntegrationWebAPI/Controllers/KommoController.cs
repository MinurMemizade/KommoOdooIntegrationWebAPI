using KommoOdooIntegrationWebAPI.Models.DTOs;
using KommoOdooIntegrationWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KommoOdooIntegrationWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KommoController : ControllerBase
    {
        private readonly IKommoService _kommoService;

        public KommoController(IKommoService kommoService)
        {
            _kommoService = kommoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeadsAsync()
        {
            var result = await _kommoService.GetLeadsAsync();
            return StatusCode(StatusCodes.Status200OK,result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeadAsync([FromForm] LeadDTO leadDTO)
        {
            await _kommoService.CreateLeadAsync(leadDTO);
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
