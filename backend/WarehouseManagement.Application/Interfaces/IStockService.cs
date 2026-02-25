using WarehouseManagement.Application.DTOs.Stock;

namespace WarehouseManagement.Application.Interfaces;

public interface IStockService
{
    Task<IEnumerable<StockDto>> GetAllStockAsync();
    Task<IEnumerable<StockDto>> GetStockByWarehouseAsync(int warehouseId);
    Task<IEnumerable<StockDto>> GetStockByProductAsync(int productId);
    Task<StockDto?> GetStockByIdAsync(int id);
    Task<StockDto> AdjustStockAsync(StockAdjustmentDto adjustmentDto);
}
