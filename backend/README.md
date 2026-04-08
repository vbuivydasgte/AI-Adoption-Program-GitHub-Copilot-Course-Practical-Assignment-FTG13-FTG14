# Warehouse Management Backend API

ASP.NET Core Web API with Clean Architecture, Entity Framework Core, and JWT Authentication.

## Architecture

- **Domain Layer**: Core entities (Product, Warehouse, Stock, History, User)
- **Infrastructure Layer**: DbContext, Repositories, Entity Configurations
- **Application Layer**: Services, DTOs, AutoMapper profiles
- **API Layer**: Controllers, Middleware, Configuration

## Setup Instructions

### Prerequisites

- .NET 10 SDK or later
- SQL Server or LocalDB

### DB Setup

1. Update connection string in `appsettings.json` if needed
2. Create and apply migrations:

```powershell
cd backend/WarehouseManagement.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../WarehouseManagement.Api
dotnet ef database update --startup-project ../WarehouseManagement.Api
```

### Run the API

```powershell
cd backend/WarehouseManagement.Api
dotnet run
```

The API will be available at `https://localhost:7262` (or the port shown in console).

### Create Initial Admin User

Use the `/api/auth/register` endpoint or run this SQL:

```sql
-- Password: Admin123
INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
VALUES ('admin', '$2a$11$xyz...', 'Admin', GETUTCDATE());
```

## API Endpoints

### Authentication

- `POST /api/auth/login` - Login with username/password
- `POST /api/auth/register` - Register new user

### Products

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product (Admin only)
- `PUT /api/products/{id}` - Update product (Admin only)
- `DELETE /api/products/{id}` - Delete product (Admin only)

### Warehouses

- `GET /api/warehouses` - Get all warehouses
- `GET /api/warehouses/{id}` - Get warehouse by ID
- `POST /api/warehouses` - Create warehouse (Admin only)
- `PUT /api/warehouses/{id}` - Update warehouse (Admin only)
- `DELETE /api/warehouses/{id}` - Delete warehouse (Admin only)

### Stock

- `GET /api/stock` - Get all stock
- `GET /api/stock/{id}` - Get stock by ID
- `GET /api/stock/warehouse/{warehouseId}` - Get stock by warehouse
- `GET /api/stock/product/{productId}` - Get stock by product
- `POST /api/stock/adjust` - Adjust stock quantity (Admin/Worker/User)

### History

- `GET /api/history/stock/{stockId}` - Get stock change history
- `GET /api/history/stock/product/{productId}` - Get stock history by product
- `GET /api/history/stock/warehouse/{warehouseId}` - Get stock history by warehouse

## Unit Tests & Coverage

Backend tests are located in `backend/WarehouseManagement.Tests` and use:

- **NUnit** for test framework
- **NSubstitute** for mocking dependencies

### Run Tests

```powershell
cd backend/WarehouseManagement.Api
dotnet test ..\WarehouseManagement.Tests\WarehouseManagement.Tests.csproj
```

### Latest Test/Coverage Report

- **Total Tests:** 38 passed, 0 failed
- **Service Coverage:**
  - `ProductService`: 100% (49/49)
  - `StockService`: 100% (86/86)
  - `HistoryService`: 100% (51/51)
  - `WarehouseService`: 100% (37/37)
  - `AuthService`: 100% (62/62)

## Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your connection string"
  },
  "Jwt": {
    "Key": "Your secret key (min 32 characters)",
    "Issuer": "WarehouseManagementApi",
    "Audience": "WarehouseManagementClient"
  }
}
```

## Technologies

- ASP.NET Core Web API
- Entity Framework Core
- AutoMapper
- JWT Bearer Authentication
- BCrypt.Net for password hashing
- SQL Server
- NUnit
- NSubstitute

## Standard Error Response Shape

Validation and middleware/auth errors now follow a consistent payload pattern:

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": {
    "FieldName": ["Error message"]
  }
}
```

For non-validation errors, payload includes:

```json
{
  "statusCode": 400,
  "message": "Business error message"
}
```
