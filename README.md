🐞 BugBoard

BugBoard is a simple bug tracking web application built with ASP.NET Core MVC. 

---

🚀 Features

Create bug reports

Edit existing reports

Delete reports with confirmation dialog

Detailed bug report view

Filter bug reports by status and priority

Color-coded priority badges

Clickable report rows for quick navigation

Automatic timestamps for created and updated reports

Responsive UI with Bootstrap



---

🛠️ Technologies

C#

ASP.NET Core MVC

Entity Framework Core

Razor Views

Bootstrap 5

SQL Server / LocalDB



---


📋 Bug Report Model

A bug report contains information such as:

Title

Description

Status

Priority

Assigned developer

CreatedAt

UpdatedAt



---

🎨 UI Features

The application uses Bootstrap badges to visually separate priorities and statuses.

Example:

Priority	Color

Low	Green
Medium	Yellow
Critical	Red



---

⚙️ Setup

1. Clone the repository

git clone https://github.com/Naceus/Bugboard.git

2. Open the project

Open the solution in:

Visual Studio or

JetBrains Rider



---

3. Apply migrations

Update-Database

or with the .NET CLI:

dotnet ef database update --project src/BugBoard.Api

---

4. Start the application

dotnet run --project src/BugBoard.Api 

