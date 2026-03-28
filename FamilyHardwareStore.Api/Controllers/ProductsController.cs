using System;
using FamilyHardwareStore.Api.Dtos;
using FamilyHardwareStore.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyHardwareStore.Api.Controllers
{
	[ApiController] // Enables API-specific conventions
	[Route("api/[controller]")] // Route template: "api/{controllerName}"
	public class ProductsController: ControllerBase
	{
		private readonly IProductService _svc;
		public ProductsController(IProductService svc)
		{
			_svc = svc;
		}

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var list = await _svc.SearchAsync(search, page, pageSize);
            // map to ProductDto quickly
            var dto = list.ConvertAll(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                NameEn = p.NameEn,
                NameMm = p.NameMm,
                Barcode = p.Barcode,
                StockQuantity = p.StockQuantity,
                SellPrice = p.SellPrice,
                ReorderLevel = p.ReorderLevel
            });
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest req)
        {
            var p = await _svc.CreateAsync(req);
            return CreatedAtAction(nameof(Get), new { id = p.Id }, p);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductRequest req)
        {
            await _svc.UpdateAsync(id, req);
            return NoContent();
        }
    }
}

