using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WarehouseManagement.Application.DTOs.Stock;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
    {
        var stocks = await _stockService.GetAllStockAsync();
        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockDto>> GetById(int id)
    {
        var stock = await _stockService.GetStockByIdAsync(id);
        
        if (stock == null)
        {
            return NotFound(new { message = "Stock record not found" });
        }

        return Ok(stock);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetByWarehouse(int warehouseId)
    {
        var stocks = await _stockService.GetStockByWarehouseAsync(warehouseId);
        return Ok(stocks);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<StockDto>>> GetByProduct(int productId)
    {
        var stocks = await _stockService.GetStockByProductAsync(productId);
        return Ok(stocks);
    }

    [HttpPost("adjust")]
    [Authorize(Roles = "Admin,Worker,User")]
    public async Task<ActionResult<StockDto>> Adjust([FromBody] StockAdjustmentDto adjustmentDto)
    {
        if (adjustmentDto.ChangedBy == string.Empty)
        {
            adjustmentDto.ChangedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }

        var stock = await _stockService.AdjustStockAsync(adjustmentDto);
        return Ok(stock);
    }
}
