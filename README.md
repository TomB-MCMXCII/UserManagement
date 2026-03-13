
# UserManagement API

## Overview
UserManagement API is a simple **.NET 9 Web API** for managing users and their profile information.  
The project demonstrates clean architecture principles, Entity Framework Core usage with PostgreSQL, and a stub message publishing mechanism.

The service exposes endpoints to:
- Create a user with a profile
- Retrieve a user with profile details

When a user is created, a stub publisher simulates sending a message by logging a JSON payload.

---

## Tech Stack

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker (database only)
- Swagger (OpenAPI)
- MSTest / VSTest

---

## Architecture

The project follows a simple layered structure to separate concerns.

```
src/
  UserManagement.Api
    Controllers        # API endpoints
    Contracts          # Request/Response DTOs
    Data               # EF Core DbContext
    Domain             # Domain entities
    Services           # Business logic + message publisher
    Migrations         # EF Core migrations

tests/
  UserManagement.Api.Tests
```

### Key Design Decisions

- **The solution is intentionally kept within a single API project to avoid unnecessary complexity for the scope of this assignment.**
- **Thin controllers** – business logic is implemented in services
- **DTO separation** – API contracts are separate from domain entities
- **Dependency injection** – services are registered through ASP.NET Core DI
- **Database migrations** – schema managed through EF Core migrations
- **Message publishing abstraction** – IMessagePublisher simulates integration with a message broker

---

## Database

The application uses **PostgreSQL**.  
A Docker container is used to run the database locally.

### docker-compose.yml

```yaml
services:
  postgres:
    image: postgres:17
    container_name: homework-postgres
    environment:
      POSTGRES_DB: homeworkdb
      POSTGRES_USER: taskuser
      POSTGRES_PASSWORD: mypassword
      PGDATA: /var/lib/postgresql/data/pgdata
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## Migrations

Entity Framework Core migrations are included in the project.

The application applies pending migrations automatically on startup via `Database.Migrate()`.

If the schema changes in the future, create a new migration with:

```bash
dotnet ef migrations add <MigrationName> --project UserManagement.Api --startup-project UserManagement.Api
```
---

## Running the Project

### 1. Start PostgreSQL

From the solution root:

```
docker compose up -d
```

### 2. Run the API

```
dotnet run --project UserManagement.Api
```

---

## API Documentation

Swagger is enabled in development mode.

Open:

```
http://localhost:<port>/swagger
```

The port number will be displayed in the console when the API starts.

---

## API Endpoints

### Get User

```
GET /api/users/{id}
```

Returns a user with profile information.

Example:

```
GET /api/users/1
```

---

### Create User

```
POST /api/users
```

Request body example:

```json
{
  "username": "jdoe",
  "email": "jdoe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1990-05-15T00:00:00"
}
```

## Example Requests

Replace <port>.

### Create user

```bash
curl -X POST http://localhost:<port>/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "jdoe",
    "email": "jdoe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "dateOfBirth": "1990-05-15T00:00:00"
  }'
```

### Get user by Id

```bash
curl http://localhost:<port>/api/users/1
```

---

## Message Publishing

After a user is successfully created, a stub publisher is called:

```
IMessagePublisher.PublishUserCreatedAsync()
```

The implementation logs a JSON payload containing:

- UserId
- Username
- Email

This simulates publishing to a message broker (e.g., RabbitMQ or Azure Service Bus).

---

## Running Tests

Run tests from the solution root:

```
dotnet test
```

Tests cover:
- Service layer behavior
- Controller responses
- Basic business logic validation

---

## Possible Improvements

If this project were extended further, potential improvements include:

- Integration tests using TestContainers
- FluentValidation for request validation
- API versioning
- Structured logging
- Authentication / authorization integration (OIDC / Keycloak)
- Real message broker integration
