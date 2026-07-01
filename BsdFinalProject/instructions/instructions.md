# Copilot Instructions for BsdFinalProject

This document provides guidelines for GitHub Copilot when working on the BsdFinalProject (.NET 9 ASP.NET Core Web API).

## Project Overview

**BsdFinalProject** is a donation/gift management system built with:
- Framework: ASP.NET Core (.NET 9)
- Architecture: Repository Pattern with Service Layer
- Database: Entity Framework Core with SaleContext
- Authentication: Authorization-based API endpoints
- API Style: RESTful with DTOs for data transfer

## Directory Structure

```
BsdFinalProject/
├── Controllers/          # API endpoint handlers (see controllers.md)
├── Repositories/         # Data access layer (see repositories.md)
├── Services/             # Business logic layer
├── Models/               # Domain entities
├── DTOs/                 # Data Transfer Objects
├── Data/                 # Database context and factories
├── Properties/           # Project metadata
└── appsettings.json      # Configuration
```

## Key Entities

- **Basket**: User shopping cart for gifts
- **Gift**: Donation items
- **Category**: Gift categorization
- **Donor**: Donation creators
- **Winner**: Gift recipients
- **Card**: Payment/transaction cards
- **User**: System users
- **Manager**: Administrative users

## General Guidelines

### Code Style
- Use async/await for all I/O operations
- Follow C# naming conventions (PascalCase for classes/methods, camelCase for variables)
- Use dependency injection via constructors
- Add XML documentation comments for public methods
- Keep lines under 120 characters

### Database Operations
- Use EntityFrameworkCore with async methods
- Always call `SaveChangesAsync()` after modifications
- Use LINQ for queries
- Handle null references properly

### API Design
- Use `[Authorize]` attribute for protected endpoints
- Return appropriate HTTP status codes (200, 201, 400, 401, 404, 500)
- Use DTOs for API requests/responses (don't expose Models directly)
- Extract userId from claims: `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`

### Error Handling
- Check for null values before processing
- Return meaningful error messages
- Use try-catch for database operations when appropriate
- Log errors appropriately

## When Working on Specific Components

- **Repositories**: See `.copilot/repositories.md` for detailed guidelines
- **Controllers**: See `.copilot/controllers.md` for detailed guidelines
- **Services**: Implement business logic, validation, and data orchestration
- **Models**: Keep as clean data containers with minimal logic
- **DTOs**: Define separate read/write DTOs as needed

## Don't Waste Tokens On

- Creating new test projects or test files (not in scope)
- Modifying appsettings.json for general questions
- Refactoring unrelated code areas
- Questions about areas outside this project scope

## Token Conservation Tips

When asking Copilot for help:
1. Ask specific questions about Repositories in the context of `.copilot/repositories.md`
2. Ask specific questions about Controllers in the context of `.copilot/controllers.md`
3. For Services, ask in main context
4. Provide the specific file names or line numbers when possible
5. Reference entity names and method signatures clearly

---

Last Updated: June 2026
