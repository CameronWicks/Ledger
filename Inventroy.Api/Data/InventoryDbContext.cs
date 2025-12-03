using Inventory.Api.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Inventory.Api.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>()
            .HasIndex(i => i.Sku)
            .IsUnique();

        modelBuilder.Entity<Item>()
            .HasMany(i => i.Transactions)
            .WithOne(t => t.Item)
            .HasForeignKey(t => t.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fixed GUIDs for seed data
        var item1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var item2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var item3Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        modelBuilder.Entity<Item>().HasData(
            new Item { Id = item1Id, Sku = "SKU-001", Name = "Blue Widget", UnitPrice = 100m, LowStockThreshold = 10 },
            new Item { Id = item2Id, Sku = "SKU-002", Name = "Red Widget", UnitPrice = 50m, LowStockThreshold = 5 },
            new Item { Id = item3Id, Sku = "SKU-003", Name = "Green Widget", UnitPrice = 75m, LowStockThreshold = 8 }
        );

        var now = DateTimeOffset.UtcNow;

        modelBuilder.Entity<StockTransaction>().HasData(
            new StockTransaction
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                ItemId = item1Id,
                QuantityChange = 50,
                Timestamp = now.AddDays(-7),
                Reference = "Initial stock"
            },
            new StockTransaction
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ItemId = item1Id,
                QuantityChange = -5,
                Timestamp = now.AddDays(-2),
                Reference = "Issue to customer"
            },
            new StockTransaction
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                ItemId = item2Id,
                QuantityChange = 20,
                Timestamp = now.AddDays(-5),
                Reference = "Initial stock"
            },
            new StockTransaction
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                ItemId = item3Id,
                QuantityChange = 5,
                Timestamp = now.AddDays(-1),
                Reference = "Initial stock"
            }
        );
    }
}
