
---

# **SOLUTION.md **

```markdown
# SOLUTION.md
## Design Decisions & Trade-offs
Inventory Ledger — Technical Assessment for Exim International (Pty) Ltd

---

## 1. Architecture Overview

The solution uses a **two-project architecture**:

1. **Inventroy.Api (Backend)**  
   - ASP.NET Core Web API (.NET 9)  
   - SQL Server + EF Core 8 with migrations  
   - Minimal APIs for clarity and reduced boilerplate  
   - GUID primary keys for best practices and distributed ID generation  

2. **Inventory (Frontend)**  
   - Blazor WebAssembly (.NET 9)  
   - Structured into services + pages  
   - Clean UI with lightweight styling  
   - Error-resilient data fetching and form validation  

This separation enables clean testing, deployment flexibility, and future extension (e.g., mobile apps, microservices).

---

## 2. Domain Model

### Item
- `Id` (Guid)
- `Sku` (string)
- `Name` (string)
- `UnitPrice` (decimal)
- `LowStockThreshold` (int)
- Navigation: `List<StockTransaction>`

### StockTransaction
- `Id` (Guid)
- `ItemId` (Guid, FK)
- `QuantityChange` (int; positive or negative)
- `Timestamp` (DateTimeOffset)
- `Reference` (string?)

### Why GUID IDs?
- Better in distributed systems
- Collisions extremely unlikely
- Decouples identity from database sequencing
- Cleaner client-side creation

---

## 3. Database & Persistence

- EF Core with SQL Server provider
- Migrations stored under `/Migrations`
- Automatic database migration on API startup (simplifies running locally)
- Decimal precision warnings addressed in model configuration
- On-hand quantity **calculated, not stored** to avoid stale data

---

## 4. API Endpoints

### Item Endpoints
- `GET /api/items?search=xyz`
- `GET /api/items/{id}`
- `POST /api/items`
- `PUT /api/items/{id}`
- `DELETE /api/items/{id}`
- `POST /api/items/{id}/transactions`
- `GET /api/items/{id}/transactions`

Validation includes:  
- SKU + Name required  
- Unique SKU constraint  
- Quantity change cannot be zero  
- ID consistency enforced  

Meaningful HTTP responses:  
- 400 BadRequest  
- 404 NotFound  
- 409 Conflict  
- 201 Created  

---

## 5. Inventory Summary Endpoint

`GET /api/inventory/summary?lowStockOnly=true`

Returns:

- Per-item summary
- On-hand quantity
- Inventory value (quantity * price)
- Low-stock flag
- Total inventory valuation
- Low-stock item count

This serves Task 3’s requirement for aggregate queries.

---

## 6. Client Application (Blazor WASM)

### Implemented Pages

- **Items List**
  - Search by SKU or name
  - Edit link
  - Transaction link

- **New Item**
  - Form with validation
  - Graceful error messages

- **Edit Item**
  - Edit form
  - Update validation
  - Navigation back to list

- **Record Transaction**
  - Increase/decrease stock
  - View transaction history

- **Inventory Summary**
  - Card layout
  - Low-stock highlighting
  - Toggle to show only low stock
  - Total value displayed

### Error Handling Philosophy
- Avoids UI crashes
- Displays actionable feedback to users
- Wraps calls in try/catch
- Shows server messages when provided

---

## 7. CORS Configuration

The API explicitly allows:

