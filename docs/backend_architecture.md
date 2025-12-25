# Backend Architecture

## Quran & Islamic Education Management App (MVP)

---

## 1. Architectural Style

The backend follows a **Clean Architecture**, implemented as a **modular monolith** using multiple projects within a single solution.

This architecture enforces:

- Clear separation of concerns
    
- Strong dependency direction
    
- Testable business logic
    
- Technology-agnostic core
    

The system is designed to scale in complexity **without requiring architectural rewrites**.

---

## 2. Solution Structure

The backend is implemented as **one solution with four projects**:

```text
QuranSchool.sln
 ├── QuranSchool.Domain
 ├── QuranSchool.Application
 ├── QuranSchool.Infrastructure
 └── QuranSchool.API
```

This structure is mandatory to enforce architectural boundaries at compile time.

---

## 3. Dependency Rules

Dependencies are strictly one-directional:

```text
Domain
  ↑
Application
  ↑
Infrastructure
  ↑
API
```

### Rules

- **Domain** references nothing
    
- **Application** references Domain only
    
- **Infrastructure** references Application and Domain
    
- **API** references Application only
    

Any violation of this direction is considered an architectural error.

---

## 4. Project Responsibilities

### 4.1 Domain Project

**Purpose:**  
Represents the core business model and rules of the system.

**Contains:**

- Entities (User, Assignment, Submission, AttendanceSession, etc.)
    
- Enums (UserRole, AttendanceStatus)
    
- Domain rules and invariants
    

**Does NOT contain:**

- EF Core
    
- ASP.NET Core
    
- DTOs
    
- Database or HTTP concepts
    

This project defines **what the system is**, not how it runs.

---

### 4.2 Application Project

**Purpose:**  
Implements business use cases and workflows.

**Contains:**

- Use cases (CreateAssignment, GradeSubmission, RecordAttendance, etc.)
    
- Interfaces (repositories, file storage, caching)
    
- Application-level validation
    
- DTOs used internally by the backend
    

**Responsibilities:**

- Enforces authorization rules at the business level
    
- Coordinates domain entities
    
- Remains independent of infrastructure details
    

**Does NOT contain:**

- EF Core implementations
    
- Controllers
    
- HTTP or framework logic
    

This project defines **what the system does**.

---

### 4.3 Infrastructure Project

**Purpose:**  
Provides technical implementations for external concerns.

**Contains:**

- EF Core `DbContext`
    
- Entity configurations
    
- Repository implementations
    
- PostgreSQL integration
    
- In-memory caching implementation
    
- File storage implementation (local or pluggable)
    

**Responsibilities:**

- Persist and retrieve data
    
- Implement interfaces defined in Application
    
- Handle external systems
    

**Notes:**

- EF Core exists **only here**
    
- Database schema aligns with DBML specification
    
- Cache invalidation occurs on write operations
    

This project defines **how the system works technically**.

---

### 4.4 API Project

**Purpose:**  
Acts as the delivery mechanism for the backend.

**Contains:**

- ASP.NET Core controllers
    
- Request/response DTOs
    
- Authentication and authorization setup
    
- Middleware
    
- Swagger/OpenAPI configuration
    

**Responsibilities:**

- Translate HTTP requests into application use cases
    
- Return appropriate HTTP responses
    
- Enforce authentication (JWT)
    

**Does NOT contain:**

- Business logic
    
- EF Core access
    
- Domain rules
    

This project defines **how the system is accessed**.

---

## 5. Technology Stack

### Backend

- **Framework:** ASP.NET Core Web API
    
- **Language:** C#
    
- **Authentication:** JWT (Bearer tokens)
    
- **Authorization:** Role-based + allocation-based filtering
    
- **Documentation:** Swagger / OpenAPI
    

---

### Data Access

- **Database:** PostgreSQL
    
- **ORM:** Entity Framework Core
    
- **Primary Keys:** UUID
    
- **Migrations:** EF Core migrations
    

---

### Caching

- **Type:** In-Memory Caching (`IMemoryCache`)
    
- **Location:** Infrastructure layer
    
- **Usage:**
    
    - Read-heavy reference data (classes, subjects, allocations)
        
- **Strategy:**
    
    - Time-based expiration
        
    - Explicit invalidation on writes
        
- **Scope:** Per application instance (non-distributed)
    

---

### File Storage

- **Purpose:** Assignment submissions (images, PDFs)
    
- **Storage:** Local server file system (MVP)
    
- **Database:** Stores file metadata and URLs only
    
- **Design:** Replaceable with cloud storage without affecting Application or Domain
    

---

## 6. Security Architecture

### Authentication

- Username + password login
    
- Passwords stored as salted hashes
    
- JWT issued on successful login
    

### Authorization

Authorization is enforced at **two levels**:

1. **API layer** – role checks
    
2. **Application layer** – business rules
    

Examples:

- Teachers can only access allocations assigned to them
    
- Students can only access their own data
    
- Parents have read-only access to linked children
    

---

## 7. Data Flow Overview

### Example: Grade Submission

1. HTTP request hits API controller
    
2. Controller validates request and user identity
    
3. Application use case is invoked
    
4. Use case enforces business rules
    
5. Repository interface is called
    
6. Infrastructure implementation persists changes
    
7. Cache is invalidated if necessary
    
8. Response returned to client
    

---

## 8. Scalability & Constraints (MVP)

- Single backend instance
    
- Stateless API
    
- No background workers
    
- No distributed cache
    
- Mobile-only clients
    

The architecture allows future extension without refactoring core logic.

---

## 9. Architectural Guarantees

This backend architecture guarantees:

- Business rules are isolated and testable
    
- Database technology can change without rewriting logic
    
- Infrastructure can evolve independently
    
- API remains thin and predictable
    
