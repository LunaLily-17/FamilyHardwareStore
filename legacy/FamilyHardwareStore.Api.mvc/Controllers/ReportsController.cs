using System;
using FamilyHardwareStore.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHardwareStore.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _svc;
        public ReportsController(IReportService svc) { _svc = svc; }

        [HttpGet("daily-sales")]
        public async Task<IActionResult> DailySales([FromQuery] DateTime? date)
        {
            var d = date ?? DateTime.UtcNow.Date;
            var report = await _svc.GetDailySalesAsync(d);
            return Ok(report);
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStock()
        {
            var list = await _svc.GetLowStockAsync();
            return Ok(list);
        }
    }

}

