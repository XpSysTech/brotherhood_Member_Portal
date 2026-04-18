# Brotherhood Portal API (.NET)

## Overview

Backend API for the Brotherhood Portal.

Built with:

* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* Swagger / OpenAPI
* GraphQL (HotChocolate)
* Serilog Logging
* Rate Limiting
* CORS

---

# 1. Requirements

Install:

* .NET SDK 8+
* Docker (recommended)
* PostgreSQL (if running outside Docker)

Check:

```bash
dotnet --version
```

---

# 2. Restore Packages

Inside API folder:

```bash
dotnet restore
```

---

# 3. Run Locally

```bash
dotnet run
```

Typical URLs:

```text
http://localhost:5001
https://localhost:7001
```

---

# 4. Configuration

Use `appsettings.json`, `appsettings.Production.json`, or environment variables.

## Required Values

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=brotherhood_db;Username=user;Password=password"
  },
  "Jwt": {
    "TokenKey": "LONG_SECRET_KEY",
    "Issuer": "Brotherhood",
    "Audience": "BrotherhoodUsers",
    "ExpiryMinutes": 15
  },
  "EnableSwagger": true
}
```

---

# 5. Environment Variables (Recommended)

```env
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=brotherhood_db;Username=boena_postgres;Password=StrongPassword123
Jwt__TokenKey=LONG_RANDOM_SECRET
Jwt__Issuer=Brotherhood
Jwt__Audience=BrotherhoodUsers
Jwt__ExpiryMinutes=15
EnableSwagger=false
ASPNETCORE_ENVIRONMENT=Production
```

---

# 6. Database Migrations

Create migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply migration:

```bash
dotnet ef database update
```

Your app may auto-run migrations at startup.

---

# 7. Swagger

Open:

```text
http://localhost:5001/swagger
```

Use for:

* testing endpoints
* login
* JWT token validation

Disable publicly in production when desired.

---

# 8. REST API Routes

Examples:

```text
/api/v1/account/login
/api/v1/members
/api/v1/finance
/api/v1/photos
```

---

# 9. GraphQL

Endpoint:

```text
/graphql
```

Example query:

```graphql
query {
  fundFinanceSummary(year: 2026, month: 4) {
    totalSavings
    totalMembers
  }
}
```

Auth required for protected queries.

---

# 10. JWT Authentication

Login returns token.

Use header:

```http
Authorization: Bearer YOUR_TOKEN
```

---

# 11. Logging

Serilog writes logs to:

```text
logs/log-yyyyMMdd.txt
```

Check runtime issues:

```bash
docker logs boena-api
```

---

# 12. Docker Build

Build image:

```bash
docker build -t boena-api -f Brotherhood_Portal.API/Dockerfile .
```

Run:

```bash
docker run -p 5001:8080 boena-api
```

---

# 13. Health Checks

Endpoint:

```text
/health
```

Use for uptime monitoring.

---

# 14. Security Checklist

* [ ] Strong JWT secret
* [ ] Swagger disabled publicly
* [ ] HTTPS enabled
* [ ] CORS restricted to frontend domain
* [ ] Rate limiting enabled
* [ ] Secrets stored in `.env`

---

# 15. Troubleshooting

## 401 Unauthorized

* token expired
* invalid issuer/audience
* missing Bearer prefix

## 404 Route Not Found

Check versioned route path:

```text
/api/v1/
```

## DB Connection Failed

Check connection string and postgres container.

## GraphQL AUTH_NOT_AUTHENTICATED

Ensure Authorization header sent.

---

# 16. Useful Commands

```bash
dotnet restore
dotnet build
dotnet run
dotnet test
dotnet ef database update
```

---

# 17. Deployment Flow

1. Pull latest code
2. Configure environment variables
3. Build container
4. Run migrations
5. Start API
6. Test Swagger
7. Test GraphQL
8. Monitor logs

---

# 18. Future Improvements

* Refresh tokens
* Audit logs
* Background jobs
* Email notifications
* CI/CD pipeline
* Integration tests
