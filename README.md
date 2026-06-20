# Planademic

A web application that helps students manage academic tasks and automatically generates a personalized weekly study schedule based on task complexity, deadlines, and saved availability.

## Features

- **Authentication** - Cookie-based login and registration with Teacher/Student roles
- **Courses** - Teachers create courses with auto-generated join codes; students enrol using a code and see the course's assignments
- **Tasks** - Students manage both course-linked assignments and personal (non-course) tasks, and can mark tasks complete
- **Availability** - Students define recurring weekly time slots via a clickable grid
- **Schedule generation** - Automatically maps pending tasks into the student's free time slots, prioritized by urgency and complexity, with no slot ever left empty
- **Dashboard** - At-a-glance overview of pending task count, tasks due within 7 days, high-complexity tasks due this week, estimated free hours, completed task count, top 5 tasks by priority, and today's scheduled sessions

## Tech Stack

| Layer          | Technology                                   |
| -------------- | --------------------------------------------- |
| Web            | ASP.NET Core Razor Pages                      |
| Business Logic | C# service classes                            |
| Data Access    | ADO.NET (`SqlConnection` / `SqlCommand`, parameterized queries) |
| Database       | Microsoft SQL Server                          |
| Testing        | xUnit with hand-written fake repositories      |

## Project Structure

```
PlanademicSolution/
├── Planademic.Domain/       # Entity models (User, Course, Assignment, StudentTask, AvailabilitySlot, CourseEnrollment, ScheduledTask)
├── Planademic.BLL/          # Services and interfaces (business logic, validation, scheduling algorithm)
├── Planademic.DAL/          # Repository interfaces and ADO.NET repository implementations
├── Planademic.Web/          # Razor Pages frontend, DI registration, authentication
└── UnitTesting/             # xUnit tests with in-memory fake repositories/services
```

Every repository and service is exposed through an interface (`IUserRepository`, `IUserService`, `ICourseRepository`, `ICourseService`, `ICourseTaskRepository`, `ICourseTaskService`, `IStudentTaskRepository`, `IStudentTaskService`, `IAvailabilityRepository`, `IAvailabilityService`, `ISchedulingService`) and wired up via ASP.NET Core's built-in dependency injection in `Program.cs`. The BLL layer never references ADO.NET directly, it only depends on DAL interfaces — which keeps business logic testable and storage swappable.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (check `global.json` / project files for the exact version targeted)
- SQL Server (local or remote instance)

### Setup

1. Clone the repository and navigate to the solution directory.

2. Create the database and required tables (`Users`, `Courses`, `CourseEnrollments`, `Assignments`, `Tasks`, `AvailabilitySlots`) on your SQL Server instance. There is no EF Core migration pipeline in this project — schema must be created manually or via a setup script, since the DAL talks to the database directly through ADO.NET.

3. Update the connection string in `Planademic.Web/appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=Planademic;Trusted_Connection=True;"
   }
   ```

4. Run the application:

   ```
   dotnet run --project Planademic.Web
   ```

The app seeds a default teacher account (`justinleomaas@gmail.com` / `password`) on first startup if one doesn't already exist.

## Scheduling Algorithm

The schedule generator (`SchedulingService`) works as follows:

1. Fetches all incomplete tasks for the student and their saved availability slots for the current Monday–Sunday week.
2. Calculates a priority score for each task:

   ```
   score = (0.4 × complexity) + (0.6 × (1 / max(daysRemaining, 1)))
   ```

   Tasks that are complex and due soon score highest. The `max(daysRemaining, 1)` guard prevents division by zero for overdue or due-today tasks.
3. Sorts tasks by priority score, highest first.
4. Determines how many slots each task should occupy: `slots = ceil(complexity / 2)`, so higher-complexity tasks claim more of the week's time.
5. Builds a weighted pool of tasks (each task repeated according to its slot count) and cycles through it to fill every available slot in date/time order, ensuring no slot is ever left empty even if the task pool is smaller than the slot count.

This logic lives entirely in `SchedulingService` behind an `ISchedulingService` interface, so it can be unit tested without a database or a live HTTP request.

## Running Tests

```
dotnet test UnitTesting/
```

Tests cover user registration, course creation/enrollment, task management, availability, and the scheduling algorithm (slot mapping, priority ordering, and edge cases such as empty availability). Each test constructs the service under test directly against a fake, in-memory implementation of the relevant repository or service interface (e.g. `FakeCourseRepository`, `FakeStudentTaskService`, `FakeAvailabilityService`), there is no mocking library involved, and no database connection is required to run the suite.

## About

Planademic is a student academic-planning project built around a layered Domain/BLL/DAL/Web architecture, with interface-based abstraction used throughout to keep business logic decoupled from data access and the UI.
