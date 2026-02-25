namespace WarehouseManagement.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    // Navigation properties
    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
