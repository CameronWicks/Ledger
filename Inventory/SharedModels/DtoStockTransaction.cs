namespace Inventory.SharedModels
{
    public class DtoStockTransaction
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int QuantityChange { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public string Reference { get; set; } = string.Empty;
    }
}
