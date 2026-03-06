# DevInsightForge Web API Template

<!-- ![DevInsightForge Logo](<URL to Logo if applicable>) -->

## Overview

This template provides a foundation for building a DevInsightForge Web API project using C#.

- **Identity**: DevInsightForge.Templates.Api
- **License**: [MIT](https://opensource.org/licenses/MIT)

## Description

This template serves as a starting point for a DevInsightForge Web API project. It includes essential structures and configurations to kickstart your development process.

## License and Policy

- License: [MIT](https://opensource.org/licenses/MIT)
- Source and package metadata include MIT license declaration and repository attribution.
- Usage policy: this template is provided "as is" under MIT terms. You are responsible for validating security, compliance, and legal requirements before production use.

## Features

- ASP.NET Core Web API project
- Clean architecture pattern
- EFCore with SqlServer
- Domain Driven Design
- Repository pattern
- JWT Authentication
- Fluent Validation
- Mapster

## Prerequisites

- [.NET SDK 10.0+ (LTS)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- `dotnet-ef` tool (for .NET CLI migration commands):

```bash
dotnet tool install --global dotnet-ef
```

## Installation

To install the DevInsightForge Web API Template, use the following command:

```bash
dotnet new install DevInsightForge.Templates.Api
```

For local development, install from this repository root:

```bash
dotnet new install .
```

## Create Solution Using Template

To create a new solution using the DevInsightForge Web API Template, use the following command:

```bash
dotnet new devforgeapi -n YourSolutionNameHere
```

## Pack Template (Maintainers)

Build a template package using the SDK-style pack project:

```bash
dotnet pack DevInsightForge.Templates.Api.csproj -c Release
```

## Manage Migrations

Use either .NET CLI or Package Manager Console.

### Apply Migrations

.NET CLI:

```bash
dotnet ef database update --project .\YourProjectName.Infrastructure --startup-project .\YourProjectName.WebAPI
```

Package Manager Console (select `YourProjectName.Infrastructure` as Default Project):

```bash
Update-Database
```

### Add New Migration

.NET CLI:

```bash
dotnet ef migrations add InitialCreate --project .\YourProjectName.Infrastructure --startup-project .\YourProjectName.WebAPI
```

Package Manager Console (select `YourProjectName.Infrastructure` as Default Project):

```bash
Add-Migration InitialCreate
```
