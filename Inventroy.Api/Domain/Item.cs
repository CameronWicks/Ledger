using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain;

public class Item
{
    public Guid Id { get; set; }

    [Required, MaxLength(64)]
    public string Sku { get; set; } = string.Empty;

    [Required, MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int LowStockThreshold { get; set; }

    public ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
}
