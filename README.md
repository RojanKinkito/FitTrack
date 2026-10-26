# FitTrack

## Fitness client management, built for clearer coaching decisions

FitTrack is an ASP.NET Core MVC fitness client management system that brings client profiles, workout programs, progress records, BMI insights, and progress charts into one secure workspace.

## Project overview

FitTrack is a portfolio-ready web application for fitness trainers and administrators. It demonstrates how a server-rendered MVC application can combine relational data, authentication, role-based authorization, validation, responsive design, and browser-based data visualization.

The application includes a public landing page for visitors and a protected dashboard for authenticated users.

## Problem the system solves

Fitness professionals often manage client information, workout plans, measurements, and progress updates across spreadsheets or disconnected tools. FitTrack provides one place to:

- Maintain client profiles and fitness goals.
- Assign structured workout programs.
- Record progress measurements over time.
- Review BMI and progress trends.
- Control access based on staff responsibilities.

## Main features

- Public landing page with product overview and sign-in actions.
- Dashboard with live client summary cards and recent clients.
- Client CRUD management with search and gender filtering.
- BMI calculation from weight and height in centimeters.
- BMI category display and live BMI preview in client forms.
- Workout program CRUD management with client assignment and search.
- Progress record CRUD management with client filtering.
- Responsive weight and body-fat progress charts using Chart.js.
- ASP.NET Core Identity registration, login, logout, and access-denied handling.
- Admin and Trainer roles with different permissions.
- Admin user-management page for assigning and removing roles.
- Development-only sample clients, programs, and progress records.
- Responsive Bootstrap frontend with a dark blue, white, and green FitTrack theme.
- Server-side and client-side validation.

FitTrack demonstrates:

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQLite
- CRUD operations
- Authentication
- Role-based authorization
- Data validation
- BMI calculation
- Chart.js data visualization
- Responsive frontend design

## User roles and permissions

| Capability | Admin | Trainer |
|---|:---:|:---:|
| View dashboard | Yes | Yes |
| View clients | Yes | Yes |
| Create clients | Yes | Yes |
| Edit clients | Yes | Yes |
| Delete clients | Yes | No |
| View workout programs | Yes | Yes |
| Create workout programs | Yes | Yes |
| Edit workout programs | Yes | Yes |
| Delete workout programs | Yes | No |
| View progress records | Yes | Yes |
| Create progress records | Yes | Yes |
| Edit progress records | Yes | Yes |
| Delete progress records | Yes | No |
| Manage user roles | Yes | No |

Trainers do not see delete controls in the interface. Restricted URLs are also protected server-side and lead to the Access Denied page.

## Technologies used

| Area | Technology |
|---|---|
| Framework | ASP.NET Core MVC on .NET 8 |
| Language | C# |
| ORM | Entity Framework Core 8 |
| Database | SQLite |
| Authentication | ASP.NET Core Identity |
| Frontend | Razor views, HTML, CSS, JavaScript |
| UI framework | Bootstrap |
| Charts | Chart.js loaded from a versioned CDN |
| Validation | Data annotations, MVC model validation, jQuery Unobtrusive Validation |

## System architecture overview

FitTrack uses a conventional ASP.NET Core MVC structure:

```text
Browser
   |
   v
Razor Views + Bootstrap + JavaScript
   |
   v
MVC Controllers
   |
   v
AppDbContext (Entity Framework Core)
   |
   v
SQLite database (fittrack.db)
```

Authentication and authorization are integrated into the request pipeline:

```text
Request
  -> Routing
  -> Authentication
  -> Role authorization
  -> Controller action
  -> Razor view
```

The main application folders are:

| Folder | Responsibility |
|---|---|
| `Controllers` | HTTP endpoints, authorization, filtering, and CRUD orchestration |
| `Data` | `AppDbContext`, Identity setup, and development seeders |
| `Models` | Domain models and view models |
| `Views` | Razor UI for each feature and shared layout |
| `Migrations` | Entity Framework database schema history |
| `wwwroot` | CSS, JavaScript, Bootstrap, and static assets |

## Database entities and relationships

The application database contains the FitTrack domain entities plus ASP.NET Core Identity tables.

```text
Client (1)
  |--< WorkoutProgram (many)
  |
  |--< ProgressRecord (many)

IdentityUser + IdentityRole
  |--< IdentityUserRole >-- IdentityRole
```

### Domain entities

| Entity | Important fields | Relationship |
|---|---|---|
| `Client` | Name, age, gender, weight, height, goal, date joined | Has many workout programs and progress records |
| `WorkoutProgram` | Program name, description, difficulty, duration | Belongs to one client through `ClientId` |
| `ProgressRecord` | Weight, body-fat percentage, record date, notes | Belongs to one client through `ClientId` |

`Client.Bmi` and `Client.BmiCategory` are calculated, non-persisted properties. Height is entered in centimeters and BMI is calculated using kilograms and meters.

## Screenshots

The following placeholders identify recommended screenshots for a portfolio presentation. Add image files under `docs/screenshots/` and update the paths when screenshots are available.

### Public landing page

<!-- Screenshot placeholder: docs/screenshots/landing-page.png -->

`[Screenshot placeholder: Public landing page]`

### Authenticated dashboard

<!-- Screenshot placeholder: docs/screenshots/dashboard.png -->

`[Screenshot placeholder: Authenticated dashboard]`

### Client management and BMI

<!-- Screenshot placeholder: docs/screenshots/client-details-bmi.png -->

`[Screenshot placeholder: Client details with BMI and progress charts]`

### Workout programs

<!-- Screenshot placeholder: docs/screenshots/workout-programs.png -->

`[Screenshot placeholder: Workout program management]`

### Progress tracking

<!-- Screenshot placeholder: docs/screenshots/progress-tracking.png -->

`[Screenshot placeholder: Progress records and charts]`

### Admin user management

<!-- Screenshot placeholder: docs/screenshots/user-management.png -->

`[Screenshot placeholder: Admin user management]`

## Installation and setup instructions

### Prerequisites

Install the following before starting:

- .NET 8 SDK
- Git
- Optional: Entity Framework Core CLI tools

Check the installed SDK:

```bash
dotnet --version
```

### Clone the repository

```bash
git clone <repository-url>
cd FitTrack
```

### Restore dependencies

```bash
dotnet restore
```

The project references the EF Core SQLite, Design, Tools, and ASP.NET Core Identity EntityFrameworkCore packages in `FitTrack.csproj`.

### Configure the database

The local development connection string is stored in `appsettings.Development.json`:

```json
"DefaultConnection": "Data Source=fittrack.db"
```

This uses a local SQLite file named `fittrack.db`. The base `appsettings.json` intentionally does not contain a database connection string, so Production must receive one through secure hosting configuration. Do not place production credentials or secrets in source-controlled configuration files.

### Configure a development Admin account

The application can create a local development Admin account from environment variables. Set these variables before starting the app; use your own local email and strong password.

PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:FITTRACK_ADMIN_EMAIL = "your-local-admin@example.com"
$env:FITTRACK_ADMIN_PASSWORD = "<choose-a-strong-local-password>"
```

Bash:

```bash
export ASPNETCORE_ENVIRONMENT=Development
export FITTRACK_ADMIN_EMAIL=your-local-admin@example.com
export FITTRACK_ADMIN_PASSWORD='<choose-a-strong-local-password>'
```

The password is read only from the environment in Development mode and is not stored in the repository.

## Deployment Preparation

FitTrack is prepared for source control and future deployment, but this repository does not push or deploy the application. Before deploying, review the target host's secret, database, HTTPS, and logging configuration.

### Production configuration

Production secrets must be provided through environment variables, a managed secret store, or secure hosting configuration. Do not commit passwords, API keys, certificates, or production connection strings to `appsettings.json`.

The application reads the database connection string using the standard .NET configuration key `ConnectionStrings:DefaultConnection`. An environment variable uses double underscores:

```text
ConnectionStrings__DefaultConnection=Data Source=/secure/data/fittrack.db
```

The current project uses the SQLite EF Core provider, so a production SQLite deployment requires a durable, writable database path and appropriate backups. A managed relational database would require adding its EF Core provider and changing the application configuration in a future deployment iteration.

Set the production environment explicitly:

```text
ASPNETCORE_ENVIRONMENT=Production
```

Also configure `AllowedHosts` for the real host name instead of leaving it as `*` in a public deployment. For example, use an environment variable or secure host configuration equivalent to:

```text
AllowedHosts=fittrack.example.com
```

HTTPS termination, certificate management, forwarded headers, secure cookie settings, and database file permissions should be configured according to the hosting platform. The application already uses HTTPS redirection and Identity cookie authentication; the reverse proxy must be configured to preserve the original HTTPS scheme.

### Production database setup

1. Provision a durable private directory for the SQLite database.
2. Set `ConnectionStrings__DefaultConnection` to the database path in the hosting environment.
3. Deploy the application files without local `fittrack.db`, `fittrack.db-shm`, or `fittrack.db-wal` files.
4. Apply the committed migrations during a controlled release step:

   ```bash
   dotnet ef database update --configuration Release
   ```

   Alternatively, run the migration command from a trusted deployment job using the published application's configuration.
5. Start the application in Production and verify login, authorization, database writes, backups, and HTTPS.

The development Admin bootstrap variables are intentionally ignored in Production. The current application creates the configured environment-based Admin account only in Development; it does not automatically create a production Admin account. A production rollout therefore needs a secure, separately planned Admin provisioning process before relying on the Admin-only user-management page.

### Repository hygiene

The repository `.gitignore` excludes build output, Visual Studio and user-specific files, local environment files, SQLite database files, logs, and test output. Source code, migrations, Razor views, static assets, project files, and `README.md` remain eligible for version control.

## How to run the project locally

Apply the database migrations first in Development:

```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet ef database update
```

Then start the application:

```bash
dotnet run
```

Open the HTTPS or HTTP URL printed by the application. If the local HTTPS certificate is not trusted, run:

```bash
dotnet dev-certs https --trust
```

When running in Development mode:

1. Identity roles (`Admin` and `Trainer`) are ensured.
2. The configured development Admin account is created or updated with the Admin role.
3. Sample domain data is inserted only when the Clients table is empty.

Registration creates a new user and assigns the Trainer role by default.

## How to apply Entity Framework migrations

Install the EF CLI tool if needed:

```bash
dotnet tool install --global dotnet-ef
```

Create a new migration after changing an entity or the database model:

```bash
dotnet ef migrations add DescribeYourChange
```

Apply pending migrations:

```bash
dotnet ef database update
```

The repository currently includes migrations for:

- Initial domain schema
- Workout programs
- Progress records
- ASP.NET Core Identity tables

Do not edit an already-applied migration. Add a new migration for subsequent schema changes.

## How development seed data works

`Data/DevelopmentDataSeeder.cs` runs only when the application environment is Development.

It inserts, when no clients exist:

- Five fictional sample clients.
- Two workout programs assigned to different clients.
- Three progress records for each of two clients.
- Realistic goals including weight loss, muscle gain, strength improvement, general fitness, and improved endurance.

The seeder checks whether any clients exist before inserting data, so it does not duplicate records on every application start. It also does not delete or overwrite existing user-created records.

To remove development seed data locally, stop the application and delete the local SQLite database file, then recreate the schema:

```bash
dotnet ef database drop --force
dotnet ef database update
```

Only do this for local development data. It permanently removes the local database contents.

## Testing and verification summary

The application has been verified through build and functional smoke testing, including:

- Successful .NET build with no compilation errors.
- Public landing page access while logged out.
- Login, registration, logout, and access-denied flows.
- Admin access to dashboard, all CRUD areas, charts, and user management.
- Trainer access to dashboard and create/edit features.
- Trainer restriction from delete actions and user management.
- Client search and gender filtering.
- Workout program search and client assignment.
- Progress filtering and chart rendering with seeded records.
- BMI calculation, categories, and invalid/empty input handling.
- Responsive navigation and mobile-oriented layout rules.
- JavaScript syntax validation for client-side behavior.

For a local verification pass, build and run:

```bash
dotnet build
dotnet run
```

Then verify the main routes manually:

```text
/
/Account/Login
/Account/Register
/Clients
/WorkoutPrograms
/ProgressRecords
/UserManagement   (Admin only)
```

## Security features

- ASP.NET Core Identity user and role management.
- Password hashing through Identity; passwords are not stored in application code.
- Cookie-based authentication with configured login and access-denied paths.
- `[Authorize]` protection on authenticated application features.
- Role-based authorization for Admin-only delete and user-management operations.
- Anti-forgery validation on state-changing POST actions.
- Local return URL validation after login.
- Server-side model validation with data annotations.
- Development-only Admin account provisioning through environment variables.
- No real personal information or production credentials in the sample seed data.

## Future improvements

Potential future enhancements include:

- Production deployment configuration with managed secrets and a hosted database.
- Pagination for large client, program, and progress-record lists.
- Richer workout exercise and set/rep planning.
- Exportable client reports.
- More granular permissions and audit logging for role changes.
- Automated integration and end-to-end browser tests.
- Accessibility testing with screen readers and automated WCAG tooling.
- Image or document attachments for progress assessments.
- Dashboard trend summaries based on progress records.

## Author

**[Your Name]**

FitTrack is presented as a portfolio project demonstrating full-stack ASP.NET Core MVC development. Replace the placeholder author name and add your preferred portfolio, GitHub, or LinkedIn links before publishing.
