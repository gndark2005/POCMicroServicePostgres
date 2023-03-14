# BuildingLink Module Service template for .NET Core

This template has the purpose to guide developers on BuildingLink’s standard way to build Event-Driven architecture Microservices. Here you will find an end-to-end example of a producer and consumer using an API as Producer and Worker service as Consumer.

If you are going to start a new microservice project, you could use this template and rename it to help you get started with the boilerplate to have API or Consumer projects that interact with events.

# Table of contents
- [Solution structure](#solution-structure)
- [Technology and Libraries used](#technology-and-libraries-used)
- [Requirements](#requirements)
- [Run Local](#run-local)
  - [Code first approach](#code-first-approach)
  - [Database first approach](#database-first-approach)
- [Use it for your module](#use-it-for-your-module)
- [Swagger documentation](#swagger-documentation)
  - [Configuration](#configuration)
- [Health check endpoints](#health-check-endpoints)
  - [Configuration](#configuration)
   - [Services](#services)
- [Authentication](#authentication)
 - [Configuration](#configuration)
- [Testing](#testing)

## Solution structure

The solution is composed of 8 projects:

-  **BuildingLink.ModuleServiceTemplate**: this project has the purpose to hold all classes and utils that could be shared among the others projects in the solution.

-  **BuildingLink.ModuleServiceTemplate.Api**: this is a .NET 6 API project with CRUD examples and event publishing.

-  **BuildingLink.ModuleServiceTemplate.Consumer**: this is a .NET 6 worker service that serves the purpose  of a consumer of some of the events published by the API.

-  **BuildingLink.ModuleServiceTemplate.Events**: this project has the purpose to hold all event classes related to your project or module.

-  **BuildingLink.ModuleServiceTemplate.Repositories.CodeFirst**: this is the project that holds the data access layer using EF core with CodeFirst migrations. If your service needs to create a database from scratch this is the project you should use to handle data access.

-  **BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst**: this is the project that holds the data access layer using EF core with DatabaseFirst migrations. If your service needs to use an existent database from scratch this is the project you should use to handle data access.

-  **BuildingLink.ModuleServiceTemplate.Services**: this project holds the business logic, here you will find the services, DTOs, mapping profiles, validators.

-  **BuildingLink.ModuleServiceTemplate.Tests**: this project holds the unit tests and integration tests for the module

## Technology and Libraries used

-  **ASP.NET Core 6**
-  **EF Core 5**
-  **SQL Server**
-  **Swagger**
-  **AutoMapper**
-  **FluentValidation**
-  **Serilog**
-  **NewRelic.LogEnrichers.Serilog**
-  **Sentry.Serilog**
-  **XUnit**
-  **Moq**
-  **BuildingLink.Messaging.MassTransit**
-  **BuildingLink.Services.Authentication**
-  **BuildingLink.Services.HealthChecks**

## Requirements

1. SQL Server

2. .NET 6 SDK

3. Visual Studio 2019 (Optional to build and edit the code.)

4. .NET Core CLI tools (Optional if you want to use it instead of VS Package Manager Console)

5. EF .NET Core CLI tools (Optional if you want to use it instead of VS Package Manager Console)

## Run Local

To run the examples in this template you need to set up the API as the startup project and then configure the database.

### Code first approach

This is the default configuration for the API.

The `BuildingLink.ModuleServiceTemplate.Repositories.CodeFirst` project is based on Entity Framework code first approach. In the next section, you'll find instructions to use it.

#### Update Database in local environment:

1. Open the `BuildingLink.ModuleServiceTemplate.Api\appsettings.Local.json` file and be sure the connection string is configured for your local computer.

```
"ConnectionStrings": {
	"CodeFirstDb": "You connection string here",
}
```
3. Open the 'Package Manager Console' in Visual Studio or use 'dotnet' tools.

4. Be sure the `BuildingLink.ModuleServiceTemplate.Repositories.CodeFirst` is selected in the Default Project dropdown in the PM Console.

5. Enter the following command `Update-Database -Verbose -Context CodeFirstDbContext` in the Package Manager Console.

#### Add new Migration after Model change in local environment:

1. Open the `BuildingLink.ModuleServiceTemplate.Api\appsettings.Development.json` file and be sure the connection string is configured for your local computer.

2. Open the 'Package Manager Console' in Visual Studio or use 'dotnet' tools.

3. Be sure the `BuildingLink.ModuleServiceTemplate.Repositories.CodeFirst` is selected in the Default Project dropdown in the PM Console.

4. Enter the following command `Add-Migration NewMigrationName -Verbose -Context CodeFirstDbContext` in the Package Manager Console.

5. After add the new migration, you need to update the Database to make persistent the new model updates. Run the update data base command.

#### Rollback:

1. Migrate to desired migration: `Update-Database -migration MigrationName`

2. Update database: `Update-Database -Verbose -Context ModuleServiceTemplateDbContext`

3. Delete old migration: You can do this manually. (Optional: Double check the context model snapshot doesn't contain the removed fields.)

For more information visit: [Database Migrations](https://buildinglink.atlassian.net/wiki/spaces/KNOW/pages/179077350/Database+Migrations)

### Database first approach

The `BuildingLink.ModuleServiceTemplate.Repositories.DatabaseFirst` project contains an example of Entity Framework database first approach.

#### Entity Framework Core Power Tools

This is a useful tool, we used to implement the database first approach. Search "EF Core Power Tools" under Extensions in Visual Studio to get this tool installed. Right-click the project and a new option named "EF Core Power Tools" should be available to start using these tools.

You can find more information in the following links:

1. https://github.com/ErikEJ/EFCorePowerTools

2. https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools#review-details

## Use it for your module

If you want to start a new project or refactor an existing one, there are a couple of things you need to do:

1. **Rename solution, projects, and folders**: you need to rename the solution file and its 8 projects with their folders from `BuildingLink.ModuleServiceTemplate.*` to `BuildingLink.[YourModuleName].*`. For example, if you are working on the Amenities module you will need to rename the projects as `BuildingLink.Amenities`, `BuildingLink.Amenities.Api`, `BuildingLink.Amenities.Consumer`, etc.

2. **Rename namespaces**: in the same way you renamed your projects, you will need to replace the namespaces of all the classes in this codebase. You can use tools like ReSharper if you have it or use the Search and replace functionality from Visual Studio Code to search and replace all occurrences for `BuildingLink.ModuleServiceTemplate.` and replace it with your module name `BuildingLink.[YourModuleName].`

3. **Remove projects and stuff you don't need**: this template has the baseline to start a project with an API and a consumer but you may not need an API in your module or a Consumer so if that is the case feel free to remove the one you are not going to use. The template also includes CRUD operations examples with their events to the consumer after you understand them you can remove everything regarding the Book entity.

4. **Rename your repositories project**: the repositories project are suffixed with CodeFirst and DatabaseFirst, it is only for demo purposes, after you have decided which approach you are going to use you can delete the other repositories project and remove the suffix from the project name. For example, if I'm going to use a CodeFirst approach I can remove the *.Repositories.DatabaseFirst project and instead of having the repositories project called like `BuildingLink.[YourModuleName].Repositories.CodeFirst` should be just `BuildingLink.[YourModuleName].Repositories`. **Note**: (This step will break the build because there are classes that were configured in the API and Consumer but it is just a matter of removing all the code that references these classes)

5. **Rename ConnectionStrings name**: for demo purposes we have 2 connection string names `CodeFirstDb` and `DatabaseFirstDb` delete/rename them with the name of the database that they connect to and make sure you update the API/Consumer to use the correct name.

7. **Ignore appsettings.Local.json file**: we included the appsettings.Local.json as part of the demo, after you are set up you need to ignore this so that it does not get published to source control with your local settings.

## Swagger documentation

The API project exposes swagger documentation as to its home page.

### Configuration

1. Open the `BuildingLink.ModuleServiceTemplate.Api\Startup.cs` file and go to the 'Swagger documentation' region.

- In this region, we are adding two middleware to generate Swagger as a JSON endpoint and to exposes a Swagger UI as a Home page.

2. Open the `BuildingLink.ModuleServiceTemplate.Api\Configuration\ServiceCollectionExtensions.cs` file and go to the 'Swagger configuration' region.

- In this region, we can register the Swagger generator defining one or more Swagger documents.

## Health check endpoints

### Configuration

Open the `BuildingLink.ModuleServiceTemplate.Api\Startup.cs` or `BuildingLink.ModuleServiceTemplate.Consumer\Startup.cs` file and go to the 'Configure' method. Inside of 'UseEndpoints' method call we are calling the .RegisterHealthProbesMapping extension method from [BuildingLink HealthCheck library](https://github.com/BuildingLink/netcore-services-healthchecks) to register the approved endpoints for the  health checks.

### Services

Open the `BuildingLink.ModuleServiceTemplate.Api\Configuration\ServiceCollectionExtensions.cs` file (the same file exists in the Consumer) and go to the 'AddHealthServices' function.

In this file, we are adding all of the necessary services for the health checks endpoints in the ASP.NET Core service collection.

## Authentication

The API project use Authentication to authorize the endpoint access, also contain some configurations to map the V2 user type node to its own roles.

### Configuration

Open the `BuildingLink.ModuleServiceTemplate.Api\Configuration\ServiceCollectionExtensions.cs` file and go to the 'AddAuthenticationServices' function.

In this function, we are configuring the fake authentication and mapping V2 user type nodes to Module Service Template roles.

## Testing

Open the solution on Visual Studio and go to the 'BuildingLink.ModuleServiceTemplate.Tests' project to run the set of tests.

This project has a set of Unit tests and another set of Integration tests.

Normally we can find the Integration tests inside of `BuildingLink.ModuleServiceTemplate.Tests\Controllers` folder.