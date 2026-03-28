using System;
using FamilyHardwareStore.Api.Dtos;
using FamilyHardwareStore.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHardwareStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _svc;
        public SalesController(ISalesService svc) { _svc = svc; }

        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
        {
            var sale = await _svc.CreateSaleAsync(request);
            return CreatedAtAction(null, new { id = sale.Id }, sale);
        }

        [HttpPost("{id:int}/refund")]
        public async Task<IActionResult> Refund(int id)
        {
            await _svc.RefundAsync(id);
            return NoContent();
        }
    }
}

