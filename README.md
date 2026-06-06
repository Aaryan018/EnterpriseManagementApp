# EnterpriseManagementApp

An ASP.NET Core MVC web application for managing enterprise operations — human resources, rentals, and events — with role-based authentication.

## Features

### Human Resources
- Employee management with attendance tracking
- Payroll processing
- Leave request handling
- Service and event management

### Rentals
- Customer and asset management
- Occupancy history tracking
- Asset invoicing
- Rent change management

### Authentication & Roles
- ASP.NET Identity with cookie-based authentication
- Three roles: **Employee**, **Manager**, **Customer**
- Role-gated access to controllers and views

## Tech Stack

| Layer          | Technology                     |
| -------------- | ------------------------------ |
| Framework      | .NET 8 (ASP.NET Core MVC)      |
| ORM            | Entity Framework Core 9        |
| Database       | SQL Server                     |
| Auth           | ASP.NET Core Identity          |
| Validation     | FluentValidation               |

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or remote)

### Configuration

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EnterpriseManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Run

```bash
dotnet restore
dotnet run
```

The app resets and re-seeds the database on every startup (development mode). Default users are created by `SeedData.cs`.

### Default URL

```
https://localhost:5001
```

You'll be redirected to the sign-in page at `/Account/SignIn`.

## Project Structure

```
EnterpriseManagementApp/
├── Controllers/       # MVC controllers (Account, HR, Rentals, Events)
├── Models/            # Domain models and view models
│   ├── HumanResource/ # Employee, Attendance, Payroll, LeaveRequest, Service, Event
│   └── Rentals/       # Customer, Asset, OccupancyHistory, AssetInvoice, RentChange
├── Views/             # Razor views
├── Migrations/        # EF Core migrations
├── DTO/               # Data transfer objects
├── SeedData.cs        # Database seeder with default users and sample data
├── ApplicationDbContext.cs  # EF Core DbContext and model configuration
└── Program.cs         # App entry point and service registration
```
