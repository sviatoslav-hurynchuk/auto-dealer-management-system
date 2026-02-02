# Programming Principles & Architecture

This document outlines the core programming principles, design patterns, and architectural decisions applied in the **Auto Dealer Management System**. The project follows a **Layered Architecture** using C# .NET, GraphQL, and ADO.NET.

## 1. SOLID Principles

### Single Responsibility Principle (SRP)
I adhere to SRP by ensuring that classes have distinct, isolated responsibilities. The project is strictly divided into layers:
* **Repositories** (`backend/Repositories`) handle direct database interactions and SQL execution.
* **Services** (`backend/Services`) handle business logic and validation.
* **GraphQL** (`backend/GraphQL`) handles the API schema, types, and DTO mapping.

**Implementation in Code:**
* **Data Access:** The `CarRepository` is solely responsible for executing SQL commands.
  * [CarRepository.cs](./backend/Repositories/CarRepository.cs)
* **Business Logic:** The `CarService` handles validation before calling the repository.
  * [CarService.cs](./backend/Services/CarService.cs)

### Dependency Inversion Principle (DIP)
High-level modules do not depend on low-level modules; both depend on abstractions. I use **Dependency Injection (DI)** throughout the application. Interfaces are defined for all repositories and services, and implementations are injected via the .NET built-in IoC container.

**Implementation in Code:**
* **Interface Definition:**
  * [ICarRepository.cs](./backend/Repositories/Interfaces/ICarRepository.cs)
* **DI Registration:**
  * [Program.cs](./backend/Program.cs) (e.g., `builder.Services.AddScoped<ICarRepository, CarRepository>(...);`)

## 2. Design Patterns

### Repository Pattern
I implement the Repository Pattern to abstract the data access layer. This allows the business logic to rely on an interface (`ICarRepository`) rather than the specific database implementation details (ADO.NET/SQL).

**Implementation in Code:**
* The repository exposes clean methods like `CreateCarAsync` or `SearchCarsAsync` while hiding the complexity of `SqlConnection` and `SqlCommand`.
  * [CarRepository.cs](./backend/Repositories/CarRepository.cs)

### Factory Pattern
To manage database connections securely and dynamically based on user roles (Admin, Manager, Reader), I implemented a `DbConnectionFactory`. This encapsulates the logic of selecting the correct connection string.

**Implementation in Code:**
* **Factory Class:**
  * [DbConnectionFactory.cs](./backend/Services/DbConnectionFactory.cs)

## 3. General Best Practices

### DRY (Don't Repeat Yourself)
I avoid code duplication by centralizing common logic. Validation logic that applies to both "Create" and "Update" operations is extracted into shared private methods.

**Implementation in Code:**
* **Shared Logic:** The `ValidateCarBase` method is reused to prevent duplicating validation rules.
  * [CarService.cs](./backend/Services/CarService.cs) (Lines referencing `ValidateCarBase`)

### Defensive Programming & Security (SQL Injection Prevention)
I prioritize security by preventing SQL Injection. Even though I use raw ADO.NET instead of a full ORM like Entity Framework, I strictly use **Parameterized Queries** instead of string concatenation.

**Implementation in Code:**
* **Parameterized Query:**
  * [CarRepository.cs](./backend/Repositories/CarRepository.cs) (e.g., `cmd.Parameters.AddWithValue("Status", filter.Status);`)

### Fail Fast
The system is designed to fail immediately if input data is invalid, throwing custom exceptions (e.g., `ValidationException`) before any expensive database operations occur.

**Implementation in Code:**
* **Guard Clauses:**
  * [CarService.cs](./backend/Services/CarService.cs) (e.g., `if (id <= 0) throw new ValidationException...`)

### Custom Exception Handling
I use custom exception types to handle different error scenarios (Validation, Not Found, Conflict), which allows for cleaner error handling in the GraphQL layer.

**Implementation in Code:**
* **Exceptions Folder:**
  * [ValidationException.cs](./backend/Exceptions/ValidationException.cs)
  * [NotFoundException.cs](./backend/Exceptions/NotFoundException.cs)
