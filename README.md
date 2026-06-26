# BugBoard

BugBoard is a web-based ticket and bug tracking application built with ASP.NET Core MVC.

The project focuses on clean CRUD workflows, role-based authorization, ticket comments, protected file attachments, activity tracking and German/English UI localization.

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
|       ├── Exceptions/      # Custom application exceptions
│       ├── Models/          # Domain and Identity models
|       ├── Resources/       # Localization resources
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

- Create and view internal comments
- 
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
- Add public comments to their own tickets
- View public comments and public activity

Reporters cannot:

- View internal comments
- Create internal comments
- View tickets created by other users

---

## Attachments

Tickets support file attachments.

Validation includes:

- Maximum file count
- Maximum file size
- Allowed file extensions
- Allowed content types
- File signature validation

Supported file types:

- PDF
- PNG
- JPG / JPEG
- WEBP

Attachments are stored outside the public `wwwroot` folder and are accessed through protected controller actions.

---

## Features

- User registration and login with ASP.NET Core Identity
- Role-based authorization with `Admin`, `Developer` and `Reporter`
- Default role seeding on application startup
- Optional initial admin user seeding via user secrets
- Ticket management with status, priority and assignee fields
- Reporters can only view their own tickets
- Admins and developers can view all tickets
- Admin-only delete permission
- Admin/developer edit permission
- Public and internal ticket comments
- Reporters can only see public comments and public activity
- Activity log for ticket changes
- File attachments for tickets
- Attachment validation by file count, file size, extension, content type and file signature
- Supported attachment types: PDF, PNG, JPG/JPEG and WEBP
- Protected inline attachment viewing
- Attachment cleanup when tickets are deleted
- Search, filtering and pagination for tickets
- AJAX-based user search in the admin area
- Admin area for user and role management
- German/English UI localization with a language switcher
- Responsive Bootstrap-based UI

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
BugReportComments
BugReportAttachments
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
- Add public and internal comments to a ticket.
- Verify that reporters cannot see internal comments.
- Upload valid attachments.
- Try uploading invalid file types.
- Open attachments through the protected attachment view.
- Switch between English and German and verify that the main UI labels update.
- Manage user roles in the admin area.

---

## Project Status

BugBoard demonstrates practical ticket management workflows with ASP.NET Core MVC, Identity, role-based authorization and clean service-oriented application structure.
