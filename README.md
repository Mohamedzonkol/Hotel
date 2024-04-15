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

MyProject
│
├── MyProject.Core       (Core domain logic and models)
│   ├── Entities         (Domain entities)
│   ├── Interfaces       (Interfaces for repositories, services, etc.)
│   └── Services         (Domain services)
│
├── MyProject.Infrastructure    (Infrastructure implementations)
│   ├── Data            (Data access layer)
│   │   ├── Migrations  (Database migrations)
│   │   └── Repositories (Concrete implementations of repositories)
│   ├── ExternalServices  (External service integrations like Stripe)
│   └── Identity        (Custom identity implementation)
│
├── MyProject.Web        (Web/UI layer)
│   ├── Controllers     (ASP.NET Core MVC controllers)
│   ├── Models          (ViewModels and DTOs)
│   ├── Views           (HTML templates)
│   └── wwwroot         (Static files like CSS, JS, images)
│
└── MyProject.Tests      (Unit and integration tests)
