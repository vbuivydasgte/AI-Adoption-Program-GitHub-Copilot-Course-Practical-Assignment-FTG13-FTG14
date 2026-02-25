namespace WarehouseManagement.Domain.Entities;

public class Stock
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<StockHistory> StockHistories { get; set; } = new List<StockHistory>();
}
