using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Repositories.Interfaces;

public interface IHistoryRepository
{
    Task AddStockHistoryAsync(StockHistory history);
    Task<IEnumerable<StockHistory>> GetAllStockHistoryAsync();
    Task<IEnumerable<StockHistory>> GetStockHistoryAsync(int stockId);
    Task<IEnumerable<StockHistory>> GetStockHistoryByProductAsync(int productId);
    Task<IEnumerable<StockHistory>> GetStockHistoryByWarehouseAsync(int warehouseId);
}
