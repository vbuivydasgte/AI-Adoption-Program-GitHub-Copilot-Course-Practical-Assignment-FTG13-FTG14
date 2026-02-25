using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Infrastructure.Repositories.Implementations;

public class HistoryRepository : IHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public HistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddStockHistoryAsync(StockHistory history)
    {
        await _context.StockHistories.AddAsync(history);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StockHistory>> GetAllStockHistoryAsync()
    {
        return await _context.StockHistories
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Product)
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Warehouse)
            .OrderByDescending(sh => sh.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockHistory>> GetStockHistoryAsync(int stockId)
    {
        return await _context.StockHistories
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Product)
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Warehouse)
            .Where(sh => sh.StockId == stockId)
            .OrderByDescending(sh => sh.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockHistory>> GetStockHistoryByProductAsync(int productId)
    {
        return await _context.StockHistories
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Product)
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Warehouse)
            .Where(sh => sh.ProductId == productId)
            .OrderByDescending(sh => sh.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockHistory>> GetStockHistoryByWarehouseAsync(int warehouseId)
    {
        return await _context.StockHistories
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Product)
            .Include(sh => sh.Stock)
                .ThenInclude(s => s.Warehouse)
            .Where(sh => sh.WarehouseId == warehouseId)
            .OrderByDescending(sh => sh.ChangedAt)
            .ToListAsync();
    }
}
