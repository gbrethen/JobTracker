# JobTracker

JobTracker is a web app for organizing job applications. It has an Angular frontend, an ASP.NET Core Web API, and a SQL Server database. The app supports viewing, searching, creating, and editing applications, with job types, work locations, and notes.

## Project structure

- `JobTrackerUI/` — Angular 19 frontend
- `JobTrackerAPI/` — ASP.NET Core 9 API using Entity Framework Core and SQL Server
- `JobTracking.sql` — SQL Server database and sample lookup data setup
- `JobTracker.sln` — Visual Studio solution

## Requirements

- .NET 9 SDK
- Node.js and npm
- SQL Server (local instance or another SQL Server you can connect to)

## Set up the database

Review `JobTracking.sql` and run it against your SQL Server to create and populate the `JobTracking` database. **The script drops and recreates its application tables**, so do not run it against a database whose data you need to keep.

The API reads the `JobTrackingDb` connection string from `JobTrackerAPI/appsettings.json`. The default connection targets a local SQL Server using Windows authentication. Update the connection string for your environment as needed. Avoid committing credentials or other secrets.

## Run the API

From the repository root:

```bash
dotnet restore JobTracker.sln
dotnet run --project JobTrackerAPI
```

In Development, the API exposes Swagger UI at `/swagger` on the URL printed by `dotnet run`.

## Run the frontend

In a separate terminal:

```bash
cd JobTrackerUI
npm install
npm start
```

Open the local URL printed by Angular CLI (usually `http://localhost:4200`). Ensure the frontend's API URL configuration points to the running API; update it if your API uses a different URL or port.

## Build

```bash
dotnet build JobTracker.sln
```

```bash
cd JobTrackerUI
npm run build
```
