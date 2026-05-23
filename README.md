# BugBoard

BugBoard is a web-based bug tracking application built with ASP.NET Core MVC.

It includes user authentication, role-based authorization, bug report management, activity logs, filtering, AJAX search and pagination.

---

## Requirements

- .NET SDK
- Git
- EF Core CLI tools
- Optional: DB Browser for SQLite or another SQLite database viewer

Install EF Core CLI tools if they are not installed yet:

```bash
dotnet tool install --global dotnet-ef
```

Check the installation:

```bash
dotnet ef --version
```

---

## Installation on Windows

Install Git with winget:

```powershell
winget install --id Git.Git -e --source winget
```

Check Git:

```powershell
git --version
```

Check the .NET SDK:

```powershell
dotnet --version
```

---

## Clone the Project

```bash
git clone https://github.com/Naceus/Bugboard.git
cd Bugboard
```

---

## Git Workflow

Recommended workflow with `develop` and feature branches:

```bash
git checkout develop
git pull origin develop
git checkout -b feature/your-feature-name
```

Commit your changes:

```bash
git add .
git commit -m "feat: describe your change"
```

Push your feature branch:

```bash
git push -u origin feature/your-feature-name
```

Then create a pull request from your feature branch into `develop` on GitHub.

---

## Project Structure

```text
Bugboard/
│
├── src/
│   └── BugBoard.Api/        # ASP.NET Core MVC application
│       ├── Controllers/     # MVC controllers
│       ├── Data/            # DbContext, migrations and seed logic
│       ├── Models/          # Domain and Identity models
│       ├── Services/        # Application services
│       ├── ViewModels/      # View-specific models
│       ├── Views/           # Razor views
│       └── wwwroot/         # CSS, JavaScript and static files
│
└── README.md
```

---

## Run the Application

Go to the application project:

```bash
cd src/BugBoard.Api
```

Restore dependencies:

```bash
dotnet restore
```

Apply database migrations:

```bash
dotnet ef database update
```

Run the application:

```bash
dotnet run
```

The application will be available at the local URL shown in the terminal.

---

## Initial Admin User

BugBoard automatically seeds the default roles on application startup:

- `Admin`
- `Developer`
- `Reporter`

Newly registered users are assigned the `Reporter` role by default.

To create an initial admin user during development, configure user secrets from the application project folder:

```bash
cd src/BugBoard.Api
dotnet user-secrets init
```

Set the admin credentials:

```bash
dotnet user-secrets set "SeedAdmin:Email" "admin@bugboard.local"
dotnet user-secrets set "SeedAdmin:Password" "Admin123!"
```

Check the configured secrets:

```bash
dotnet user-secrets list
```

After starting the application, the admin user will be created automatically if it does not already exist.

The admin password is not stored in `appsettings.json` and should not be committed to the repository.

---

## Roles and Permissions

### Admin

Admins can:

- View all bug reports
- Create bug reports
- Edit bug reports
- Delete bug reports

### Developer

Developers can:

- View all bug reports
- Create bug reports
- Edit bug reports

### Reporter

Reporters can:

- Create bug reports
- View their own bug reports
- View details for their own bug reports

Reporters cannot edit or delete bug reports.

---

## Features

- User registration and login with ASP.NET Core Identity
- Role-based authorization with `Admin`, `Developer` and `Reporter`
- Default role seeding on application startup
- Optional initial admin user seeding via user secrets
- Bug reports are linked to the user who created them
- Reporters can only view their own bug reports
- Admins and developers can view all bug reports
- Admin-only delete permission
- Admin/developer edit permission
- Bug report management with status, priority and assignee fields
- Activity log for bug report changes
- Search, filtering and pagination for bug reports
- AJAX-based bug report search
- Bootstrap-based responsive UI

---

## Technologies

- C#
- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- Razor Views
- Bootstrap 5
- JavaScript / AJAX

---

## Database

The project uses SQLite.

Important tables:

```text
BugReports
BugReportLogs
AspNetUsers
AspNetRoles
AspNetUserRoles
```

The local database file should not be committed.

After changing the data model, create a new migration:

```bash
dotnet ef migrations add MigrationName
```

Apply the migration:

```bash
dotnet ef database update
```

---

## Manual Testing

After setup, check the following cases:

- Log in with the seeded admin user.
- Register a normal user and verify that the user receives the `Reporter` role.
- Create bug reports with different users.
- Verify that reporters only see their own bug reports.
- Verify that reporters cannot open details pages for bug reports created by other users.
- Verify that admins and developers can see all bug reports.
- Verify that only admins can delete bug reports.
- Verify that admins and developers can edit bug reports.

---

## Project Status

BugBoard is currently a learning and portfolio project focused on ASP.NET Core MVC, Identity, role-based authorization and clean CRUD workflows.
