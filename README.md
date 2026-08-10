# GameStore API

> ⚠️ **Learning project.** This repository exists to practice building a minimal API with .NET, Entity Framework Core, and automated testing. It is not intended for production use, and design decisions favor clarity/learning over completeness (e.g. no auth, minimal validation, no logging/observability yet).

## What this project is

A small game catalog API built with **ASP.NET Core Minimal APIs**, backed by **SQLite** via **EF Core**, with a matching **NUnit** test suite. The goal is to practice:

- Minimal API endpoint organization (grouped, static endpoint handlers)
- EF Core: `DbContext`, migrations, querying, bulk operations
- Central Package Management (CPM) across multiple projects in one solution
- Unit testing endpoint handlers directly against an in-memory SQLite database
- NUnit 4.x conventions (constraint-based assertions, multiple-assert scopes)

## Solution structure

```
GameStore/
├──
├── GameStore.Api/                 # the API project
│   ├── Data/                      # DbContext, migrations
│   ├── Dtos/                      # request/response DTOs
│   ├── Endpoints/                 # minimal API endpoint groups (e.g. GamesEndpoints)
│   ├── Models/                    # entities (Game, Genre)
│   ├── GameStore.slnx
│   ├── Directory.Packages.props   # project managed package versions
│   └── GameStore.Api.csproj
└── GameStore.Api.Test/            # test project
    ├── GameStoreTestBase.cs       # shared SQLite in-memory DB setup/teardown
    ├── GameStoreDataBuilder.cs    # test data seeding helpers
    ├── Endpoints/                 # tests per endpoint group
    ├── Directory.Packages.props   # project managed package versions
    └── GameStore.Api.Test.csproj
```

## Tech stack

| Layer                | Tech                                                                                          |
| -------------------- | --------------------------------------------------------------------------------------------- |
| Runtime / SDK        | .NET 10                                                                                       |
| Web framework        | ASP.NET Core Minimal APIs (`Microsoft.NET.Sdk.Web`)                                           |
| ORM                  | Entity Framework Core 10                                                                      |
| Database             | SQLite (`Microsoft.EntityFrameworkCore.Sqlite`) — file-based for the app, in-memory for tests |
| Migrations tooling   | `Microsoft.EntityFrameworkCore.Design`                                                        |
| Test framework       | NUnit 4 (`NUnit`, `NUnit3TestAdapter`, `NUnit.Analyzers`)                                     |
| Test runner/coverage | `Microsoft.NET.Test.Sdk`, `coverlet.collector`                                                |
| Package management   | Central Package Management (`Directory.Packages.props`), one file for the whole solution      |
| IDE / tooling used   | JetBrains Rider / ReSharper (per build output), VS Code                                       |

## Endpoints

| Method | Route          | Description                      |
| ------ | -------------- | -------------------------------- |
| GET    | `/games`       | List all games (with genre name) |
| GET    | `/games/{id}`  | Get a single game by id          |
| POST   | `/games`       | Create a new game                |
| PUT    | `/games/{id}`  | Update an existing game          |
| DELETE | `/games/{id}`  | Delete a game                    |
| GET    | `/genres`      | List all genres                  |
| GET    | `/genres/{id}` | Get a single genre by id         |
| POST   | `/genres`      | Create a new genre               |
| PUT    | `/genres/{id}` | Update an existing genre         |
| DELETE | `/genres/{id}` | Delete a genre                   |

## Running the API

```bash
dotnet restore GameStore.Api
dotnet run --project GameStore.Api
```

## Running the tests

```bash
dotnet test GameStore.Api.Test
```

Tests spin up a fresh **in-memory SQLite database per test** and call endpoint handler methods directly — no HTTP server is started.

## Notes / things intentionally left simple (for now)

- **No integration tests yet.** Current tests call endpoint methods directly rather than going through `WebApplicationFactory` + real HTTP requests, so routing, model binding, and middleware aren't covered yet. This is a natural next step.
- **No authentication/authorization.**
- **Minimal input validation** on DTOs.
- **No logging or error-handling middleware** beyond default framework behavior.

## Possible next steps

- [ ] Add `WebApplicationFactory`-based integration tests
- [x] Add a `Genres` endpoint group + tests
- [ ] Add centralized exception handling
- [x] Add OpenAPI/Swagger UI for manual exploration
