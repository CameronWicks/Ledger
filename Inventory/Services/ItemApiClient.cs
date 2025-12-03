using Inventory.SharedModels;
using System.Net.Http.Json;

namespace Inventory.Services;

public class ItemApiClient
{
    private readonly HttpClient _http;

    public ItemApiClient(HttpClient http) => _http = http;

    public async Task<List<DtoItemList>> GetItemsAsync(string? search = null)
    {
        var url = "api/items";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"?search={Uri.EscapeDataString(search)}";

        return await _http.GetFromJsonAsync<List<DtoItemList>>(url) ?? new();
    }

    public Task<DtoItem?> GetItemAsync(Guid id) =>
        _http.GetFromJsonAsync<DtoItem>($"api/items/{id}");

    public async Task<bool> CreateItemAsync(DtoItem dto, List<string> errors)
    {
        var response = await _http.PostAsJsonAsync("api/items", dto);
        if (response.IsSuccessStatusCode) return true;
        errors.Add(await response.Content.ReadAsStringAsync());
        return false;
    }

    public async Task<bool> UpdateItemAsync(DtoItem dto, List<string> errors)
    {
        var response = await _http.PutAsJsonAsync($"api/items/{dto.Id}", dto);
        if (response.IsSuccessStatusCode) return true;
        errors.Add(await response.Content.ReadAsStringAsync());
        return false;
    }

    public Task<HttpResponseMessage> DeleteItemAsync(Guid id) =>
        _http.DeleteAsync($"api/items/{id}");

    public async Task<bool> RecordTransactionAsync(DtoStockTransaction dto, List<string> errors)
    {
        var response = await _http.PostAsJsonAsync($"api/items/{dto.ItemId}/transactions", dto);
        if (response.IsSuccessStatusCode) return true;
        errors.Add(await response.Content.ReadAsStringAsync());
        return false;
    }

    public Task<List<DtoStockTransaction>?> GetTransactionsAsync(Guid itemId) =>
        _http.GetFromJsonAsync<List<DtoStockTransaction>>($"api/items/{itemId}/transactions");
}
