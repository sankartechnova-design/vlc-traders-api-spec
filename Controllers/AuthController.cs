# VLC Traders Business Manager API

This repository contains a starter ASP.NET Core Web API implementation for the VLC Traders Business Manager specification.

## Stack
- ASP.NET Core Web API
- Entity Framework Core
- MySQL / MariaDB
- JWT authentication
- Swagger/OpenAPI

## Features
- Customer, vendor, material, purchase, sale, stock, expense, CRM, quotation, invoice, note, task, bank ledger, and audit models
- MySQL EF Core configuration
- Standard API response envelope
- JWT auth setup
- Global exception middleware
- Swagger security configuration

## Configuration
Update the connection string and JWT settings in `appsettings.json` before running the project.

## Run
```bash
dotnet restore
dotnet build
dotnet run
```

Then open:
- http://localhost:5000/swagger
- or the configured ASP.NET Core HTTPS port

## Sample login
POST /api/v1/auth/login

Request body:
```json
{
  "username": "admin",
  "password": "Password@123"
}
```

## Notes
This is a starter backend foundation, designed to be extended with full ERP workflows, validation, and production-grade service layers.
