# CCMS Backend

This is the Clean Architecture backend for the CCMS project, built with .NET 10.

## Structure
- `src/CCMS.Domain`: Enterprise logic and Entities
- `src/CCMS.Application`: Business logic and Use Cases
- `src/CCMS.Infrastructure`: Database, Entity Framework, External APIs
- `src/CCMS.API`: Web API Entry Point
- `tests/`: Unit and Integration tests

## How to Run
1. Navigate to the `src/CCMS.API` folder.
2. Run `dotnet run`.
