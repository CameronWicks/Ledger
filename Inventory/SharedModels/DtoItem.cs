namespace Inventory.SharedModels
{
    public class DtoItem
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int LowStockThreshold { get; set; }
    }

    public class DtoItemList : DtoItem
    {
        public int OnHandQuantity { get; set; }
    }
}
