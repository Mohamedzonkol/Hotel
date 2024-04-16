# Hotel Project

MyProject is a .NET 8 web application built with Clean Architecture, integrating various features such as ASP.NET Core MVC, Entity Framework, Stripe Payment Integration, Repository Pattern, etc.

## Features

- **ASP.NET MVC Core (.NET 8)**: Utilize MVC pattern for building web applications.
- **Clean Architecture**: Organize codebase into layers for separation of concerns and maintainability.
- **Entity Framework with Code First Migrations**: ORM for database operations with code-first approach for migrations.
- **Stripe Payment Integration**: Integrate Stripe API for handling payments.
- **Repository Pattern**: Abstract database access to promote decoupling and testability.
- **Seed Database Migrations Automatically**: Seed initial data using Entity Framework migrations.
- **Deploying on MyWindowsHosting**: Deploy the website on your Windows hosting provider.
- **Dynamic PPT/PDF/Word Exports**: Generate dynamic exports in various formats.
- **Charts in .NET Core**: Visualize data using charting libraries compatible with .NET Core.
- **Admin Dashboard**: Implement a dashboard for administrative tasks.
- **Custom .NET Identity using MVC**: Customize identity management using ASP.NET Core Identity with MVC.

## Project Structure

This structure separates concerns into layers:

- **Applicatiion**: Contains the core domain logic, including entities representing the business domain, interfaces defining contracts for repositories and services, and services implementing domain-specific logic.
- **Domain Entities**: These are the building blocks of the application, representing the business objects and rules.
- **Interfaces**: Defines contracts for repositories, services, and other components, promoting loose coupling and dependency inversion.
- **Services**: Implements domain-specific logic, encapsulating behavior that operates on domain entities.
- **Infrastructure**: Provides implementations for data access, external service integrations, and identity management. It includes the data access layer with database migrations and repository implementations.
- **Web/UI**: Houses the web/UI layer, including controllers for handling HTTP requests, models for view models and DTOs, views containing HTML templates, and wwwroot for static files.
- 
This modular structure promotes maintainability, testability, and scalability of the Hotel Project.




