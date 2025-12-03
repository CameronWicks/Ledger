using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain;

public class StockTransaction
{
    public Guid Id { get; set; }

    [Required]
    public Guid ItemId { get; set; }

    public Item Item { get; set; } = default!;

    [Range(-1000000, 1000000)]
    public int QuantityChange { get; set; }

    [Required]
    public DateTimeOffset Timestamp { get; set; }

    [MaxLength(256)]
    public string Reference { get; set; } = string.Empty;
}
