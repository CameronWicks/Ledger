using Inventory;
using Inventory.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7011") // <--- API URL from the logs
});

// Register your API clients
builder.Services.AddScoped<ItemApiClient>();
builder.Services.AddScoped<InventoryApiClient>();

await builder.Build().RunAsync();
