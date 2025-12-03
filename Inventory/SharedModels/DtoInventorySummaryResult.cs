namespace Inventory.SharedModels
{
    public class DtoInventorySummaryResult
    {
        public List<DtoInventorySummaryItem> Items { get; set; } = new();
        public decimal TotalInventoryValue { get; set; }
        public int LowStockCount { get; set; }
    }
}
