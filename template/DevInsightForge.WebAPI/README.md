# DevInsightForge.WebAPI

## Quick Start

From solution root:

```bash
dotnet restore
```

```bash
dotnet build DevInsightForge.slnx
```

```bash
dotnet run --project DevInsightForge.WebAPI\DevInsightForge.WebAPI.csproj
```

## Development Commands

Create migration:

```bash
dotnet ef migrations add <MigrationName> --project DevInsightForge.Persistence --startup-project DevInsightForge.WebAPI
```

Apply migration:

```bash
dotnet ef database update --project DevInsightForge.Persistence --startup-project DevInsightForge.WebAPI
```

Remove last migration:

```bash
dotnet ef migrations remove --project DevInsightForge.Persistence --startup-project DevInsightForge.WebAPI
```
