using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs.History;
using WarehouseManagement.Application.Interfaces;

namespace WarehouseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("stock")]
    public async Task<ActionResult<IEnumerable<StockHistoryDto>>> GetAllStockHistory()
    {
        var history = await _historyService.GetAllStockHistoryAsync();
        return Ok(history);
    }

    [HttpGet("stock/{stockId}")]
    public async Task<ActionResult<IEnumerable<StockHistoryDto>>> GetStockHistory(int stockId)
    {
        var history = await _historyService.GetStockHistoryAsync(stockId);
        return Ok(history);
    }

    [HttpGet("stock/product/{productId}")]
    public async Task<ActionResult<IEnumerable<StockHistoryDto>>> GetStockHistoryByProduct(int productId)
    {
        var history = await _historyService.GetStockHistoryByProductAsync(productId);
        return Ok(history);
    }

    [HttpGet("stock/warehouse/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<StockHistoryDto>>> GetStockHistoryByWarehouse(int warehouseId)
    {
        var history = await _historyService.GetStockHistoryByWarehouseAsync(warehouseId);
        return Ok(history);
    }
}
