using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Repositories.Interfaces;

public interface IWarehouseRepository : IGenericRepository<Warehouse>
{
    Task<Warehouse?> GetByIdWithStocksAsync(int id);
}
