using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Infrastructure.Repositories.Implementations;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByIdWithStocksAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Stocks)
                .ThenInclude(s => s.Warehouse)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetBySKUAsync(string sku)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.SKU == sku);
    }
}
