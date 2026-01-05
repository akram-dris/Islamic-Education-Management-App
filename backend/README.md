# ⚙️ Education Management Assahaba Backend

This directory contains the core logic of the Education Management Assahaba, built with **.NET 10** following **Clean Architecture** principles.

## 🏗️ Architecture

The backend is structured to ensure maintainability and testability:

1.  **Domain**: Core entities, enums, and domain logic. No external dependencies.
2.  **Application**: Use cases, MediatR commands/queries, and interfaces for infrastructure.
3.  **Infrastructure**: Implementation of persistence (EF Core), file storage, and external services.
4.  **API**: ASP.NET Core Web API controllers, middleware, and authentication.

### 🛡️ Core Features
*   **Global Soft Delete**: All entities inherit from a base `Entity` class and are never physically deleted.
*   **Automated Auditing**: `CreatedAt`, `CreatedBy`, `LastModifiedAt`, and `LastModifiedBy` are handled automatically by the `ApplicationDbContext`.
*   **CQRS Pattern**: Clear separation between Read and Write operations using MediatR.

## 🚀 Running Locally

### Configuration
1.  Copy `.env.example` to `.env` and fill in your secrets.
2.  Update `appsettings.json` in `QuranSchool.API` if you're not using the default Docker setup.

### Commands
*   **Build**: `dotnet build`
*   **Run**: `dotnet run --project QuranSchool.API`
*   **Test**: `dotnet test`
*   **Add Migration**: 
    ```bash
    dotnet ef migrations add <Name> --project QuranSchool.Infrastructure --startup-project QuranSchool.API
    ```

## 🧪 Testing Strategy
*   **Unit Tests**: Focus on business rules in `Domain` and `Application`.
*   **Integration Tests**: Test the full stack from API to Database using `BaseIntegrationTest`.

## 📚 API Endpoints
Once the API is running, access the interactive Swagger documentation at:
`http://localhost:5000/swagger` (or your configured port).
