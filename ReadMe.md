# LearnCsla - Multitenancy POC

This project is a Proof of Concept (POC) demonstrating a robust multitenancy architecture using **CSLA .NET 10** and **Managed Extensibility Framework (MEF)**.

## Key Features

- **Dynamic Rule Discovery**: Uses MEF to discover and register business rules for all tenants into the CSLA rule cache during global initialization.
- **Rule Partitioning**: Leverages CSLA `RuleSets` to isolate business logic between different tenants.
- **Dependency Injection**: Fully integrated with .NET Dependency Injection for service resolution (Tenant Resolvers, Rule Managers).
- **Source Generators**: Utilizes CSLA 10 source generators (`[CslaImplementProperties]`) for clean, boilerplate-free business objects.
- **Asynchronous Data Access**: All data portal operations are implemented using `async/await` patterns.

## Project Structure

- **LearnCsla.BusinessLibrary**: Contains the core business objects (`Organisation`, `TenantInfo`) and multitenancy infrastructure.
- **LearnCsla.Dal**: Defines the data access interfaces and DTOs.
- **LearnCsla.DalSqlite**: (Placeholder) for SQLite implementation of the DAL.
- **LearnCsla.Tests.UnitTests**: XUnit tests verifying tenant isolation and rule application.

## Multitenancy Implementation

The system resolves the `TenantId` from the user's `ClaimsPrincipal`. Upon the first access of a business type, the `DynamicRuleManager` scans the assembly for exported rules decorated with `[ExportRule]` metadata and populates the global CSLA cache partitioned by the tenant identifier.

Individual instances then set their `BusinessRules.RuleSet` to the resolved tenant ID, ensuring that only the relevant rules are executed for that specific tenant context.
