# Warehouse Management System — Prompt & Workflow Log

## 1) Overview of the App

- Full-stack warehouse management system for products, warehouses, stock levels, and stock-change history.
- Frontend: React + TypeScript + Vite with role-aware routing and separate main/admin UX.
- Backend: ASP.NET Core Web API with layered architecture, EF Core, JWT auth, and business-rule validation.
- Key quality outcomes: reusable frontend abstractions, standardized API error contracts, and fully passing backend tests.

## 2) Project Context Provided to AI

- Build a small full-stack Warehouse Management app.
- Frontend: React + TypeScript + Vite + React Router, with separate Main/Admin layouts.
- Backend: ASP.NET Core Web API + EF Core + layered architecture.
- Core features: Products/Warehouses CRUD, stock management, history tracking, auth/roles.
- Constraints used during development:
  - Keep architecture clean and scalable.
  - Refactor for readability, consistency, validation, and performance.
  - Add backend unit tests with NUnit + NSubstitute.

### Acceptance approach used

- Copilot-generated changes were accepted.
- When refinements were needed, they were requested through additional follow-up prompts to Copilot.

## 3) Models, Tools, and External Utilities

### AI models/tools used

- GitHub Copilot in VS Code (primary coding assistant).
- Copilot Chat (iterative debugging/refactoring).
- GPT-5.3-Codex model in this session.
- ChatGPT (external) for planning/brainstorming in earlier phases.

### MCP / external tools used

- No MCP servers were used.
- EF Core CLI: migrations and DB update workflows.
- Swagger/Swashbuckle: endpoint verification and API docs.
- NUnit + NSubstitute: backend test creation and verification.

## 4) Key Prompts in Order (with outcomes)

### Step 1 — Architecture-first kickoff

- **Prompt used**
  - Initial prompt (exact):

    > I want to build a small full-stack Warehouse Management application.
    >
    > Frontend:
    >
    > - React
    > - TypeScript
    > - React Router
    > - Vite
    > - Multiple layouts (MainLayout and AdminLayout)
    >
    > Backend:
    >
    > - ASP.NET Core Web API (.NET 8)
    > - Entity Framework Core
    > - Clean architecture (Controllers → Services → Repositories → DbContext)
    > - SQL database
    > - History tracking for entity changes
    >
    > Application Requirements:
    >
    > - Manage Products (CRUD)
    > - Manage Warehouses (CRUD)
    > - Stock management (quantity per warehouse)
    > - View history of changes for products and stock
    > - Admin section with separate layout
    > - Routing with nested routes
    >
    > IMPORTANT:
    > Do NOT generate implementation code.
    >
    > Only provide:
    >
    > 1. Frontend folder structure (tree format)
    > 2. Backend folder structure (tree format)
    > 3. High-level explanation of:
    >    - Routing strategy (React Router)
    >    - Layout separation (Main vs Admin)
    >    - Backend layering
    >    - How history tracking should be structured (entity history or audit table)
    > 4. How frontend communicates with backend
    >
    > Keep the architecture clean, scalable, and production-ready.
    > Avoid overengineering.

- **Result (summary)**
  - Produced frontend/backend architecture blueprint and communication flow.
- **Accepted and follow-up prompts (if needed)**
  - Accepted overall structure.
  - Follow-up prompts were used later to refine naming/details to match implementation constraints.

### Step 2 — Auth hook/import fix

- **Prompt used**
  - “Module has no exported member `useAuth`; fix export and imports everywhere.”
- **Result (summary)**
  - Added correct hook export path and aligned imports across app.
- **Accepted and follow-up prompts (if needed)**
  - Accepted fully to remove repeated module errors.
- **Code example**

  ```ts
  // src/context/useAuth.ts
  export { useAuth } from "./authContext.config";

  // usage
  import { useAuth } from "../context/useAuth";
  ```

### Step 3 — Swagger enablement

- **Prompt used**
  - “Add swagger documentation on the backend.”
- **Result (summary)**
  - Added Swashbuckle setup and development Swagger UI.
- **Accepted and follow-up prompts (if needed)**
  - Accepted fully for easier API validation during development.
- **Code example**

  ```csharp
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();

  app.UseSwagger();
  app.UseSwaggerUI();
  ```

### Step 4 — Seed + migration startup

- **Prompt used**
  - “Add seeding for admin/worker and apply migrations on app start; run commands.”
- **Result (summary)**
  - Added startup migration flow and initial user seed logic.
- **Accepted and follow-up prompts (if needed)**
  - Accepted; follow-up prompts were used to resolve EF pending-model-change issues.
- **Code example**
  ```csharp
  using (var scope = app.Services.CreateScope())
  {
      var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      await db.Database.MigrateAsync();
  }
  ```

### Step 5 — Admin CRUD usability fixes

- **Prompt used**
  - “Admin add product/warehouse buttons do not work.”
- **Result (summary)**
  - Replaced placeholders with functional forms and API submit handlers.
- **Accepted and follow-up prompts (if needed)**
  - Accepted; follow-up prompts added loading/error handling for better UX.
- **Code example**
  ```tsx
  const handleCreate = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();
    setSubmitting(true);
    await productService.create({
      name,
      sku,
      description: description || undefined,
    });
    await loadProducts();
  };
  ```

### Step 6 — Worker stock flow

- **Prompt used**
  - “Worker should manage stock; use dropdowns instead of ID inputs.”
- **Result (summary)**
  - Implemented stock adjustment flow with product/warehouse selectors.
- **Accepted and follow-up prompts (if needed)**
  - Accepted to reduce user errors and improve task flow.
- **Code example**
  ```tsx
  <select
    value={productId}
    onChange={(e) => setProductId(e.target.value)}
    required
  >
    <option value="">Select Product</option>
    {products.map((product) => (
      <option key={product.id} value={String(product.id)}>
        {product.name} ({product.sku})
      </option>
    ))}
  </select>
  ```

### Step 7 — Authorization + server validation

- **Prompt used**
  - “Worker requests return forbidden; enforce non-negative stock result in backend.”
- **Result (summary)**
  - Updated role authorization and added service guard for negative stock.
- **Accepted and follow-up prompts (if needed)**
  - Accepted with explicit `400` behavior for invalid adjustments.
- **Code example**

  ```csharp
  [HttpPost("adjust")]
  [Authorize(Roles = "Admin,Worker,User")]
  public async Task<ActionResult<StockDto>> Adjust([FromBody] StockAdjustmentDto adjustmentDto)
  {
    var stock = await _stockService.AdjustStockAsync(adjustmentDto);
    return Ok(stock);
  }

  private static void EnsureNonNegativeQuantity(int quantity)
  {
    if (quantity < 0)
      throw new InvalidOperationException(ValidationMessages.StockQuantityCannotBeNegative);
  }
  ```

### Step 8 — UI resilience and readability

- **Prompt used**
  - “Do not hide forms/tables when stock update fails; improve responsive/contrast.”
- **Result (summary)**
  - Error became inline/non-blocking; layout/table responsiveness and contrast improved.
- **Accepted and follow-up prompts (if needed)**
  - Accepted to keep workflow usable during validation failures.
- **Code example**
  ```tsx
  {
    error && <div className={styles.errorAlert}>{error}</div>;
  }
  {
    user && <form onSubmit={handleAdjustStock}>...</form>;
  }
  <DataTable data={sortedStocks} columns={columns} keyField="id" />;
  ```

### Step 9 — Navigation and history completion

- **Prompt used**
  - “Hide protected nav when logged out; remove admin dashboard; history not showing data.”
- **Result (summary)**
  - Conditional navigation, dashboard removal, and fully wired history page.
- **Accepted and follow-up prompts (if needed)**
  - Accepted; follow-up prompts included route cleanup and redirect behavior.
- **Code example**

  ```tsx
  {user && (
    <Link to={ROUTES.STOCK} style={{ color: "white", textDecoration: "none" }}>
      Stock
    </Link>
  )}

  <Route path={ROUTES.ADMIN.HISTORY} element={<HistoryPage />} />
  <Route path="/admin/dashboard" element={<Navigate to={ROUTES.ADMIN.PRODUCTS} replace />} />
  ```

### Step 10 — Contract/schema change

- **Prompt used**
  - “Remove warehouse capacity.”
- **Result (summary)**
  - Removed capacity across entity/DTO/migrations/frontend forms/views.
- **Accepted and follow-up prompts (if needed)**
  - Accepted as a complete cross-layer contract simplification.
- **Code example**
  ```csharp
  public class Warehouse
  {
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;
      public string Location { get; set; } = string.Empty;
  }
  ```

### Step 11 — Table standardization + sorting

- **Prompt used**
  - “Add sorting on all tables and make admin lists tables.”
- **Result (summary)**
  - Unified table UI and sorting behavior across pages.
- **Accepted and follow-up prompts (if needed)**
  - Accepted for consistency and usability.
- **Code example**
  ```ts
  const { sortedData, handleSort, sortIndicator } = useSortableData<Product>(
    products,
    {
      defaultSortField: "name" as keyof Product,
      defaultSortDirection: "asc",
    },
  );
  ```

### Step 12 — Business rule hardening

- **Prompt used**
  - “Do not allow duplicate product SKUs in backend.”
- **Result (summary)**
  - Added duplicate checks on create/update and mapped violations to `400`.
- **Accepted and follow-up prompts (if needed)**
  - Accepted to enforce a critical domain rule.
- **Code example**
  ```csharp
  var existingProductWithSku = await _productRepository.GetBySKUAsync(createDto.SKU);
  if (existingProductWithSku != null)
  {
      throw new InvalidOperationException(ValidationMessages.ProductSkuAlreadyExists);
  }
  ```

### Step 13 — Remove unneeded history scope

- **Prompt used**
  - “Remove product history tables/endpoints/UI; keep stock history.”
- **Result (summary)**
  - Removed product-history paths across backend/frontend/schema.
- **Accepted and follow-up prompts (if needed)**
  - Accepted to match actual business requirement.
- **Code example**
  ```ts
  export const historyService = {
    getAllStockHistory: async (): Promise<StockHistory[]> => {
      const response =
        await axiosInstance.get<StockHistory[]>("/history/stock");
      return response.data;
    },
  };
  ```

### Step 14 — Test and coverage phase

- **Prompt used**
  - “Implement backend unit tests with NUnit/NSubstitute; raise coverage.”
- **Result (summary)**
  - Added service-layer test suite and expanded branch coverage.
- **Accepted and follow-up prompts (if needed)**
  - Accepted; final status: **38 passed, 0 failed**, targeted service coverage maintained at 100%.
- **Code example**
  ```csharp
  [Test]
  public void AdjustStockAsync_WhenNewStockWouldBeNegative_ThrowsInvalidOperationException()
  {
      var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.AdjustStockAsync(adjustment));
      Assert.That(ex!.Message, Is.EqualTo("Stock quantity cannot be negative."));
  }
  ```

### Step 15 — Refactoring program (key outcome)

- **Prompt used**
  - “Examine code and propose refactoring opportunities focused on readability, types, error handling, validation, performance.”
  - Then iterative “continue/yes” prompts to execute plan steps.
- **Result (summary)**
  - Completed major refactors: hooks/components/service factories, backend CRUD base, error contract standardization, DI cleanup, validation consolidation, async/consistency polish.
- **Accepted and follow-up prompts (if needed)**
  - Accepted; follow-up prompts were used iteratively to apply refinements while keeping behavior stable.
- **Code example**
  ```csharp
  public abstract class CrudServiceBase<TEntity, TDto>
  {
    protected async Task<IEnumerable<TDto>> GetAllAsync() { ... }
    protected async Task<TDto?> GetByIdAsync(int id) { ... }
    protected async Task<TDto> CreateAsync<TCreateDto>(TCreateDto createDto, Func<TCreateDto, Task>? validateBeforeCreate = null) { ... }
    protected async Task<TDto?> UpdateAsync<TUpdateDto>(int id, TUpdateDto updateDto, Action<TEntity, TUpdateDto> applyUpdates, Func<int, TEntity, TUpdateDto, Task>? validateBeforeUpdate = null) { ... }
    protected async Task<bool> DeleteAsync(int id) { ... }
  }
  ```

## 5) Refactoring Snapshot (final)

- Security: BCrypt migration with legacy compatibility.
- Performance: stock/history N+1 elimination.
- Safety: transaction-protected stock adjustments.
- Consistency:
  - standardized API error responses,
  - standardized frontend error/validation handling,
  - reusable table/sorting/data-fetch hooks.
- Maintainability:
  - backend CRUD base,
  - frontend CRUD service factory,
  - CSS modules migration,
  - centralized DI/startup extensions,
  - centralized validation messages/helpers.

## 6) Insights

### Which prompts worked well

- Prompts with exact error text + expected behavior.
- Narrow, scope-bounded prompts per module/file.
- Iterative “fix → validate → continue” prompting.

### Which prompts did not work well (and why)

- Very broad prompts without explicit scope (“update all files/check all”) caused partial or uneven updates.
- Prompts lacking concrete acceptance criteria made results harder to verify.

### Best prompting patterns observed

- Start with architecture, then implement feature-by-feature.
- For bugs: provide symptom, exact error, expected outcome.
- For refactors: request plan first, then execute in ordered steps with tests after each phase.

### Recommendations for future use of Copilot

- Keep prompts scoped to one feature/fix at a time with explicit acceptance criteria.
- Include concrete runtime/compiler errors when requesting fixes.
- Ask for “implement + validate” to ensure code changes are tested, not just suggested.
- For large refactors, request a step plan first, then execute step-by-step with checkpoints.
- Maintain a short prompt/outcome log during development to speed up later documentation and reviews.

---

This document is intentionally concise and focused on key prompts, decisions, and outcomes.
