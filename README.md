# InventoryLedger

A lightweight inventory ledger system built as a technical assessment for Exim International (Pty) Ltd.

This solution consists of:

- **Inventroy.Api** — ASP.NET Core Web API (SQL Server, EF Core, .NET 9)
- **Inventory** — Blazor WebAssembly Client (standalone .NET 9)

The system allows operations users to capture stock movements, view current on-hand quantities, track low-stock items, and see inventory valuation.  
All data is stored in SQL Server using EF Core with GUID primary keys.

---

## Features

### API (Inventroy.Api)
- CRUD management for **Items**
- Record **stock transactions** (increase or decrease)
- Automatic calculation of **on-hand quantity**
- **Inventory summary endpoint**:
  - Per-item valuation
  - On-hand quantity
  - Low-stock detection
  - Total inventory value
- EF Core migrations with SQL Server
- CORS enabled for Blazor client
- Automatic database migration on startup
- Seeded sample items for quick testing

### Blazor WebAssembly Client (Inventory)
- Browse and search items by SKU or name
- Create and edit items
- Record stock transactions for an item
- View inventory summary with:
  - Total inventory value
  - Low-stock filtering toggle
- Clean, easy-to-use UI
- Graceful error handling
- Typed API client services with `HttpClient`

---

## Requirements

- .NET 9 SDK
- SQL Server (LocalDB or full SQL Server instance)
- Visual Studio 2022 or VS Code

---

## Running the Solution Locally

### 1️⃣ Restore & build both projects
Open the solution folder and run:

```bash
dotnet build
