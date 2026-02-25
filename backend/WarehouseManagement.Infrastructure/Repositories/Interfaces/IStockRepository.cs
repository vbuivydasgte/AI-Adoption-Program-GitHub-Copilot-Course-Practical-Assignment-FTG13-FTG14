using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Repositories.Interfaces;

public interface IStockRepository : IGenericRepository<Stock>
{
    Task<IEnumerable<Stock>> GetAllWithDetailsAsync();
    Task<Stock?> GetByProductAndWarehouseAsync(int productId, int warehouseId);
    Task<IEnumerable<Stock>> GetByWarehouseIdAsync(int warehouseId);
    Task<IEnumerable<Stock>> GetByProductIdAsync(int productId);
    Task<Stock?> GetByIdWithDetailsAsync(int id);
}
