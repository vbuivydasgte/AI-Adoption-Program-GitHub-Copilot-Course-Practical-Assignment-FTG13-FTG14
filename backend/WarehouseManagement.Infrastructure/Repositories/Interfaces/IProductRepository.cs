using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByIdWithStocksAsync(int id);
    Task<Product?> GetBySKUAsync(string sku);
}
