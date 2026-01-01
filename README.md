# 🕌 Islamic Education Management System (IEMS)

[![.NET 10](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-green.svg)](docs/backend_architecture.md)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-MVP-orange.svg)]()

A modern, robust, and secure management platform designed specifically for Quranic schools and Islamic education centers. This system streamlines communication and academic tracking between administrators, teachers, students, and parents.

## 🚀 Overview

IEMS provides a unified ecosystem to manage the complexities of educational allocations, assignment tracking, and attendance monitoring, all while adhering to industry-standard architectural patterns.

### 🎭 Key Personas
*   **Admins**: Full control over academic structure, users, and allocations.
*   **Teachers**: Manage assignments, grade submissions, and track attendance.
*   **Students**: Access learning materials, submit assignments, and track progress.
*   **Parents**: Monitor children's academic performance and attendance records.

## 🛠️ Technology Stack

| Layer | Technology |
| :--- | :--- |
| **Backend** | .NET 10, C#, ASP.NET Core Web API |
| **Architecture** | Clean Architecture, CQRS with MediatR |
| **Persistence** | PostgreSQL, Entity Framework Core |
| **Security** | JWT Authentication, Role-Based Access Control (RBAC) |
| **Testing** | xUnit, FluentAssertions, Testcontainers |
| **Docs** | Swagger/OpenAPI, DBML |

## 📂 Repository Structure

```text
.
├── backend/                # .NET 10 Modular Monolith
│   ├── QuranSchool.API            # Entry point & Controllers
│   ├── QuranSchool.Application    # Use Cases & Business Logic
│   ├── QuranSchool.Domain         # Entities & Core Rules
│   ├── QuranSchool.Infrastructure # Persistence & External Services
│   ├── QuranSchool.UnitTests      # Domain & Application Logic Tests
│   └── QuranSchool.IntegrationTests # API & Infrastructure Tests
├── docs/                   # Detailed Documentation & Specifications
└── .github/                # CI/CD Workflows
```

## 🚥 Getting Started

### Prerequisites
*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   [Docker](https://www.docker.com/) (for PostgreSQL)

### Quick Start
1.  **Clone & Navigate**
    ```bash
    git clone https://github.com/akram-dris/Islamic-Education-Management-App.git
    cd Islamic-Education-Management-App
    ```
2.  **Start Database**
    ```bash
    cd backend
    docker-compose up -d
    ```
3.  **Run Migrations & Start API**
    ```bash
    dotnet ef database update --project QuranSchool.Infrastructure --startup-project QuranSchool.API
    dotnet run --project QuranSchool.API
    ```

## 📖 Documentation
Detailed technical guides are located in the [`/docs`](docs/README.md) directory:
*   [Backend Architecture](docs/backend_architecture.md)
*   [API Specification](docs/api_specification.md)
*   [Database Schema](docs/database_schema.md)

## 🤝 Contributing
Please read our contribution guidelines before submitting Pull Requests.

## 📄 License
This project is licensed under the MIT License - see the LICENSE file for details.