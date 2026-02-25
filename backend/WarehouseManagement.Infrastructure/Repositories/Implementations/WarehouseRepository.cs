using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Infrastructure.Repositories.Implementations;

public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Warehouse?> GetByIdWithStocksAsync(int id)
    {
        return await _dbSet
            .Include(w => w.Stocks)
                .ThenInclude(s => s.Product)
            .FirstOrDefaultAsync(w => w.Id == id);
    }
}
