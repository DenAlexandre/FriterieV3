# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FriterieShop (FriterieV3) is an e-commerce application built with **Blazor WebAssembly** frontend and **ASP.NET Core Web API** backend, targeting **.NET 10.0**, with **PostgreSQL** as the database and **Microsoft Aspire** for orchestration.

## Build & Run Commands

```bash
# Restore and build entire solution
dotnet build FriterieShop.sln

# Run the API (from repo root)
dotnet run --project FriterieShop.Presentation/FriterieShop.API

# Run the Blazor WebAssembly frontend
dotnet run --project FriterieShop.Presentation/FriterieShop.Web

# Run via Aspire AppHost (orchestrates all services + PostgreSQL)
dotnet run --project FriterieShop.AppHost

# Run tests
dotnet test FriterieShop.Tests

# Run a single test class
dotnet test FriterieShop.Tests --filter "FullyQualifiedName~ProductServiceTests"

# Run a single test method
dotnet test FriterieShop.Tests --filter "FullyQualifiedName~ProductServiceTests.GetAllAsync_ShouldReturnProducts"

# Docker Compose (full stack)
docker-compose up --build
```

## Architecture

Clean Architecture with four layers. Dependencies flow inward: Presentation → Infrastructure → Application → Domain.

```
FriterieShop.sln
├── FriterieShop.Domain              # Entities, enums, repository interfaces (Contracts/)
├── FriterieShop.Application         # Services, DTOs, validation, AutoMapper profiles
├── FriterieShop.Infrastructure      # EF Core DbContext, repository implementations, JWT, Stripe, email
├── FriterieShop.Presentation/
│   ├── FriterieShop.API             # ASP.NET Core Web API controllers
│   ├── FriterieShop.Web             # Blazor WebAssembly SPA (Tailwind CSS via npm)
│   └── FriterieShop.Web.Shared      # Shared DTOs, service interfaces, HTTP helpers, API route constants
├── FriterieShop.ServiceDefaults     # Aspire shared config (health checks, OpenTelemetry, resilience)
├── FriterieShop.AppHost             # Microsoft Aspire orchestrator (targets net8.0, not net10.0)
└── FriterieShop.Tests               # xUnit + Moq tests for Application services
```

### Key Patterns

- **Repository pattern**: `IGenericRepository<T>` in Domain/Contracts returns `int` (rows affected) from mutating methods; services check `> 0` for success
- **Service layer**: Application services return `ServiceResponse` records. Interface in `Application/Services/Contracts/`, implementation in `Application/Services/`
- **DTOs**: Separate Create/Update/Get DTOs per entity in Application/DTOs, mapped via AutoMapper (`MappingConfig.cs`)
- **Validation**: FluentValidation validators in Application/Validations, executed through `IValidationService` which concatenates errors with `"; "`
- **Auth**: JWT Bearer (HS256, 2-hour expiry) + ASP.NET Identity with refresh tokens stored in DB. Three roles: Admin, User, Cuisine. First registered user automatically becomes Admin
- **DI registration**: Each layer has a `DependencyInjection.cs` extension method (`AddApplication`, `AddInfrastructure`)

### Database

- **PostgreSQL** via EF Core (Npgsql provider)
- **No EF migrations** — uses `db.Database.EnsureCreated()` at startup with retry logic. Schema changes require dropping and recreating the DB in dev
- **DbContext**: `AppDbContext` in Infrastructure/Data, extends `IdentityDbContext<AppUser>`
- **Seeded data**: Payment methods (4) and Identity roles (Admin, User, Cuisine) in `OnModelCreating`; `SampleDataSeeder` runs at startup if Categories table is empty
- **Connection string**: `ConnectionStrings:DefaultConnection` in appsettings.json

### Important Gotchas

- **Dual `ServiceResponse` types**: Application layer has `ServiceResponse` with `object? Payload`, while Web.Shared has its own `ServiceResponse` with `JsonElement? Payload`. These are separate types that must be kept in sync manually
- **GenericRepository eager-loading hack**: `GenericRepository.GetAllAsync()` and `GetByIdAsync()` contain `typeof(TEntity) == typeof(Product)` checks to auto-include `Variants`. If adding a new entity that needs eager loading, either add another type check or create a specialized repository (preferred)
- **Hardcoded localhost URLs**: Stripe success/cancel URLs, email confirmation links, and PayPal URLs all hardcode `localhost:7258`. These need env-var injection for production
- **PayPal is a stub**: `PayPalPaymentService` returns a hardcoded URL and always captures successfully

### Frontend (Blazor WASM)

HTTP call chain: `Blazor Page → IXxxService → IApiCallHelper.ApiCallTypeCall<T>(ApiCall) → IHttpClientHelper → Named HttpClient "Blazor-Client"` (with `RefreshTokenHandler` for auto token refresh on 401).

- API route constants centralized in `Web.Shared/Constant.cs`
- Auth state managed by `CustomAuthStateProvider` with JWT parsing from browser cookies
- Styling: Tailwind CSS (npm install + build runs on every `dotnet build` via MSBuild target)

### API Structure

Controllers in `FriterieShop.API/Controllers/` use `[Route("api/[controller]")]`:
- Routes: `/api/{resource}/{action}` (e.g., `/api/product/all`, `/api/cart/checkout`)
- Authorization: Product/Category reads are public; writes require `Admin`. Cart checkout requires `User,Admin,Cuisine`. Metrics require `Admin`
- Global error handling via `ExceptionHandlingMiddleware` (catches `DbUpdateException` → 409, others → 500)
- File uploads served from `ContentRootPath/uploads` at `/uploads`

## Adding a New Feature (End-to-End)

1. **Domain**: Add entity in `Domain/Entities/`, add interface in `Domain/Contracts/`
2. **Infrastructure**: Add `DbSet<>` to `AppDbContext`, implement repository in `Infrastructure/Repositories/`, register in `Infrastructure/DependencyInjection.cs`
3. **Application**: Add DTOs in `Application/DTOs/`, add AutoMapper mappings in `MappingConfig.cs`, add service interface in `Application/Services/Contracts/`, implement in `Application/Services/`, register in `Application/DependencyInjection.cs`
4. **API**: Add controller in `API/Controllers/`
5. **Frontend**: Add mirror DTOs/service interface in `Web.Shared/`, implement client service in `Web.Shared/Services/`, add route constants to `Constant.cs`, register service in `Web/Program.cs`, add Razor pages/components in `Web/Pages/`

## Testing

Tests in `FriterieShop.Tests/` mirror two layers:
- `Application/Services/` — tests for server-side Application services
- `Presentation/Services/` — tests for Web.Shared client services

Pattern: constructor-based setup with Moq, xUnit `[Fact]`/`[Theory]`, naming convention `MethodName_Condition_ExpectedBehavior`.

## Configuration

Key appsettings.json sections: `ConnectionStrings`, `Jwt` (Key/Issuer/Audience), `Stripe`, `BankTransfer`, `EmailSettings`, `Recommendations`. See `.env.example` for required secrets.

## Language

Code comments and some UI text are in **French**. Variable names, class names, and API routes are in English.
