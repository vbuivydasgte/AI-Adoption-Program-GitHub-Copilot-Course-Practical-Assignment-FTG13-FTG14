using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Application.DTOs.Warehouses;

public class UpdateWarehouseDto
{
    [Required(ErrorMessage = "Warehouse name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Warehouse name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Location must be between 1 and 200 characters")]
    public string Location { get; set; } = string.Empty;
}
