using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQL Server
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS so the Blazor WASM app can call this API
const string CorsPolicy = "AllowClient";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7277",
                "http://localhost:7277"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors(CorsPolicy);


// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Inventory API (.NET 9, SQL Server, GUID keys)");

MapItemEndpoints(app);
MapInventoryEndpoints(app);

app.Run();

// ---------- Endpoints with GUIDs ----------

static void MapItemEndpoints(WebApplication app)
{
    // list + search
    app.MapGet("/api/items", async (InventoryDbContext db, string? search) =>
    {
        var query = db.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(i =>
                i.Sku.ToLower().Contains(s) ||
                i.Name.ToLower().Contains(s));
        }

        var items = await query
            .Select(i => new
            {
                i.Id,
                i.Sku,
                i.Name,
                i.UnitPrice,
                i.LowStockThreshold,
                OnHandQuantity = i.Transactions.Sum(t => t.QuantityChange)
            })
            .ToListAsync();

        return Results.Ok(items);
    });

    // get single
    app.MapGet("/api/items/{id:guid}", async (InventoryDbContext db, Guid id) =>
    {
        var item = await db.Items.FindAsync(id);
        return item is null ? Results.NotFound() : Results.Ok(item);
    });

    // create
    app.MapPost("/api/items", async (InventoryDbContext db, Item item) =>
    {
        if (string.IsNullOrWhiteSpace(item.Sku) || string.IsNullOrWhiteSpace(item.Name))
            return Results.BadRequest("SKU and Name are required.");

        var exists = await db.Items.AnyAsync(i => i.Sku == item.Sku);
        if (exists)
            return Results.Conflict($"Item with SKU '{item.Sku}' already exists.");

        // Ensure GUID if client sent empty
        if (item.Id == Guid.Empty)
            item.Id = Guid.NewGuid();

        db.Items.Add(item);
        await db.SaveChangesAsync();
        return Results.Created($"/api/items/{item.Id}", item);
    });

    // update
    app.MapPut("/api/items/{id:guid}", async (InventoryDbContext db, Guid id, Item updated) =>
    {
        if (id != updated.Id)
            return Results.BadRequest("ID mismatch.");

        var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.Sku = updated.Sku;
        item.Name = updated.Name;
        item.UnitPrice = updated.UnitPrice;
        item.LowStockThreshold = updated.LowStockThreshold;

        await db.SaveChangesAsync();
        return Results.NoContent();
    });

    // delete
    app.MapDelete("/api/items/{id:guid}", async (InventoryDbContext db, Guid id) =>
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.NoContent();
    });

    // record transaction
    app.MapPost("/api/items/{id:guid}/transactions", async (InventoryDbContext db, Guid id, StockTransaction dto) =>
    {
        if (id != dto.ItemId)
            return Results.BadRequest("ItemId must match route id.");

        if (dto.QuantityChange == 0)
            return Results.BadRequest("QuantityChange cannot be 0.");

        var itemExists = await db.Items.AnyAsync(i => i.Id == id);
        if (!itemExists) return Results.NotFound("Item not found.");

        if (dto.Id == Guid.Empty)
            dto.Id = Guid.NewGuid();

        if (dto.Timestamp == default)
            dto.Timestamp = DateTimeOffset.UtcNow;

        db.StockTransactions.Add(dto);
        await db.SaveChangesAsync();
        return Results.Created($"/api/items/{id}/transactions/{dto.Id}", dto);
    });

    // list transactions
    app.MapGet("/api/items/{id:guid}/transactions", async (InventoryDbContext db, Guid id) =>
    {
        var itemExists = await db.Items.AnyAsync(i => i.Id == id);
        if (!itemExists) return Results.NotFound("Item not found.");

        var tx = await db.StockTransactions
            .Where(t => t.ItemId == id)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();

        return Results.Ok(tx);
    });
}

static void MapInventoryEndpoints(WebApplication app)
{
    app.MapGet("/api/inventory/summary", async (InventoryDbContext db, bool lowStockOnly = false) =>
    {
        var query =
            from i in db.Items
            let onHand = i.Transactions.Sum(t => t.QuantityChange)
            select new InventorySummaryDto
            {
                ItemId = i.Id,
                Sku = i.Sku,
                Name = i.Name,
                UnitPrice = i.UnitPrice,
                OnHandQuantity = onHand,
                InventoryValue = onHand * i.UnitPrice,
                LowStock = onHand < i.LowStockThreshold
            };

        if (lowStockOnly)
        {
            query = query.Where(x => x.LowStock);
        }

        var items = await query.ToListAsync();
        var totalValue = items.Sum(x => x.InventoryValue);

        var result = new InventorySummaryResultDto
        {
            Items = items,
            TotalInventoryValue = totalValue,
            LowStockCount = items.Count(x => x.LowStock)
        };

        return Results.Ok(result);
    });
}

// DTOs
public class InventorySummaryDto
{
    public Guid ItemId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int OnHandQuantity { get; set; }
    public decimal InventoryValue { get; set; }
    public bool LowStock { get; set; }
}

public class InventorySummaryResultDto
{
    public List<InventorySummaryDto> Items { get; set; } = new();
    public decimal TotalInventoryValue { get; set; }
    public int LowStockCount { get; set; }
}
