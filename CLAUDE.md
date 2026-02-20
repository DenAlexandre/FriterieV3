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
├── FriterieShop.AppHost             # Microsoft Aspire orchestrator (.NET 8.0)
└── FriterieShop.Tests               # xUnit + Moq tests for Application services
```

### Key Patterns

- **Repository pattern**: `IGenericRepository<T>` in Domain/Contracts, implementations in Infrastructure/Repositories
- **Service layer**: Application services (e.g., `ProductService`, `AuthenticationService`) orchestrate business logic, return `ServiceResponse` records
- **DTOs**: Separate Create/Update/Get DTOs per entity in Application/DTOs, mapped via AutoMapper (`MappingConfig.cs`)
- **Validation**: FluentValidation validators in Application/Validations, executed through `IValidationService`
- **Auth**: JWT Bearer + ASP.NET Identity with refresh tokens. Three roles: Admin, User, Cuisine
- **DI registration**: Each layer has a `DependencyInjection.cs` extension method (`AddApplication`, `AddInfrastructure`)

### Database

- **PostgreSQL** via EF Core (Npgsql provider)
- **DbContext**: `AppDbContext` in Infrastructure/Data, extends `IdentityDbContext<AppUser>`
- **Seeded data**: Payment methods and Identity roles configured in `OnModelCreating`; sample products/categories via `SampleDataSeeder`
- **Connection string**: `ConnectionStrings:DefaultConnection` in appsettings.json

### Frontend (Blazor WASM)

- HTTP calls go through `IHttpClientHelper` → `IApiCallHelper` abstractions in Web.Shared
- API route constants centralized in `Web.Shared/Constant.cs`
- Auth state managed by `CustomAuthStateProvider` with JWT parsing
- Auto token refresh via `RefreshTokenHandler` (HTTP message handler)
- Styling: Tailwind CSS (built via npm on publish/build)

### API Structure

Controllers in `FriterieShop.API/Controllers/` follow RESTful conventions:
- Routes: `/api/{resource}/{action}` (e.g., `/api/product/all`, `/api/cart/checkout`)
- Authorization via `[Authorize(Roles = "Admin")]` attributes
- Global error handling via `ExceptionHandlingMiddleware`
- File uploads served from `wwwroot/uploads`

## Testing

Tests live in `FriterieShop.Tests/Application/Services/` mirroring the Application service structure. Pattern: mock repositories with Moq, test services directly, use xUnit `[Fact]` and `[Theory]`.

## Configuration

Key appsettings.json sections: `ConnectionStrings`, `Jwt` (Key/Issuer/Audience), `Stripe`, `BankTransfer`, `EmailSettings`, `Recommendations`. See `.env.example` for required secrets.

## Language

Code comments and some UI text are in **French**. Variable names, class names, and API routes are in English.
