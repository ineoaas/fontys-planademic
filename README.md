# Planademic

A web application that helps students manage academic tasks and automatically generates a personalized study schedule based on their weekly availability, task deadlines, and complexity.

## Features

- **Authentication** — Cookie-based login and registration with Teacher/Student roles
- **Courses** — Teachers create courses with join codes; students enroll and view assignments
- **Tasks** — Students manage both course-linked assignments and personal tasks
- **Availability** — Students define recurring weekly time slots for studying
- **Schedule generation** — Automatically fills availability slots with tasks, prioritized by urgency and complexity

## Tech Stack

| Layer | Technology |
|---|---|
| Web | ASP.NET Core Razor Pages |
| Business Logic | C# service classes |
| Data Access | Entity Framework Core |
| Database | Microsoft SQL Server |
| Testing | xUnit with fake repositories |

## Project Structure

```
PlanademicSolution/
├── Planademic.Domain/       # Entity models (User, Course, Assignment, StudentTask, AvailabilitySlot)
├── Planademic.BLL/          # Services and interfaces (business logic)
├── Planademic.DAL/          # EF Core DbContext and repository implementations
├── Planademic.Web/          # Razor Pages frontend
└── UnitTesting/             # xUnit tests with in-memory fake repositories
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote instance)

### Setup

1. Clone the repository and navigate to the solution directory.

2. Update the connection string in `Planademic.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=Planademic;Trusted_Connection=True;"
   }
   ```

3. Apply migrations to create the database:
   ```bash
   dotnet ef database update --project Planademic.DAL --startup-project Planademic.Web
   ```

4. Run the application:
   ```bash
   dotnet run --project Planademic.Web
   ```

The app seeds a default teacher account (`justinleomaas@gmail.com` / `password`) on first startup.

## Scheduling Algorithm

The schedule generator (`SchedulingService`) works as follows:

1. Fetches all incomplete tasks for the student.
2. Calculates a priority score for each task:
   ```
   score = (0.4 × complexity) + (0.6 × (1 / daysRemaining))
   ```
   Tasks that are complex and due soon score highest.
3. Assigns tasks to the student's availability slots for the current week, distributing more slots to higher-complexity tasks (a complexity-10 task receives up to 5 slots).
4. Cycles through the task pool to ensure no slot is ever left empty.

## Running Tests

```bash
dotnet test UnitTesting/
```

Tests cover user registration, course enrollment, task management, and availability, using fake in-memory repositories to stay isolated from the database.
