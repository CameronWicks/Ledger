namespace Inventory.SharedModels
{
    public class DtoInventorySummaryItem
    {
        public Guid ItemId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int OnHandQuantity { get; set; }
        public decimal InventoryValue { get; set; }
        public bool LowStock { get; set; }
    }
}
