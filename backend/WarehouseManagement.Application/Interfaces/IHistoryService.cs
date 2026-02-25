using WarehouseManagement.Application.DTOs.History;

namespace WarehouseManagement.Application.Interfaces;

public interface IHistoryService
{
    Task<IEnumerable<StockHistoryDto>> GetAllStockHistoryAsync();
    Task<IEnumerable<StockHistoryDto>> GetStockHistoryAsync(int stockId);
    Task<IEnumerable<StockHistoryDto>> GetStockHistoryByProductAsync(int productId);
    Task<IEnumerable<StockHistoryDto>> GetStockHistoryByWarehouseAsync(int warehouseId);
}
