namespace WarehouseManagement.Domain.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    // Navigation properties
    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
