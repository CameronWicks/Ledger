using Inventory.SharedModels;
using System.Net.Http.Json;

namespace Inventory.Services;

public class InventoryApiClient
{
    private readonly HttpClient _http;

    public InventoryApiClient(HttpClient http) => _http = http;

    public Task<DtoInventorySummaryResult?> GetSummaryAsync(bool lowStockOnly = false)
    {
        var url = "api/inventory/summary";
        if (lowStockOnly)
            url += "?lowStockOnly=true";

        return _http.GetFromJsonAsync<DtoInventorySummaryResult>(url);
    }
}
