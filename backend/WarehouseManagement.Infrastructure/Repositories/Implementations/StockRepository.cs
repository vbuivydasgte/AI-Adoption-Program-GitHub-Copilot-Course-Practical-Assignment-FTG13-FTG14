using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Infrastructure.Repositories.Implementations;

public class StockRepository : GenericRepository<Stock>, IStockRepository
{
    public StockRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Stock>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .ToListAsync();
    }

    public async Task<Stock?> GetByProductAndWarehouseAsync(int productId, int warehouseId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
    }

    public async Task<IEnumerable<Stock>> GetByWarehouseIdAsync(int warehouseId)
    {
        return await _dbSet
            .Include(s => s.Product)
            .Where(s => s.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetByProductIdAsync(int productId)
    {
        return await _dbSet
            .Include(s => s.Warehouse)
            .Where(s => s.ProductId == productId)
            .ToListAsync();
    }

    public async Task<Stock?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
