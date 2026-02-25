using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseDto>> GetById(int id)
    {
        var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
        
        if (warehouse == null)
        {
            return NotFound(new { message = "Warehouse not found" });
        }

        return Ok(warehouse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseDto createWarehouseDto)
    {
        var warehouse = await _warehouseService.CreateWarehouseAsync(createWarehouseDto);
        return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<WarehouseDto>> Update(int id, [FromBody] UpdateWarehouseDto updateWarehouseDto)
    {
        var warehouse = await _warehouseService.UpdateWarehouseAsync(id, updateWarehouseDto);
        
        if (warehouse == null)
        {
            return NotFound(new { message = "Warehouse not found" });
        }

        return Ok(warehouse);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _warehouseService.DeleteWarehouseAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = "Warehouse not found" });
        }

        return NoContent();
    }
}
