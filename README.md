# TaskManagementAPI

This is a .NET core project running on .Net version 8, Entity Framework Core and PostgreSQL. The API provides user registration/login (JWT-based) and basic task CRUD(Create,Read,Update and Delete). The appsettings.json has all the important configuration entries from database connection, Jwt entries settings.

Essentials before you get started;
You need a Docker Application.
Pull the postgress Image in the docker Application by searching for it.
Once you have the postgres image , Execute the following commands in your docker terminal.
1.create a container for the application to hold the database which will be used in the this codebase.
docker run --name my-postgres \-e POSTGRES_USER=postgres \-e POSTGRES_PASSWORD=data \-e POSTGRES_DB=TasksDatabase \-v pgdata:/var/lib/postgresql/data \-p 5432:5432 \ -d postgres
Check your containers for my-postgres container to confirm it has been created
2.Execute docker ps to check if the container is running
Expected output is here
docker ps
CONTAINER ID IMAGE COMMAND CREATED STATUS PORTS NAMES
c92244240228 postgres "docker-entrypoint.s…" About a minute ago Up About a minute 0.0.0.0:5432->5432/tcp, [::]:5432->5432/tcp my-postgres 3. Install the client to enable you have access to psl by executing
sudo apt install postgresql-client -y
4.Execute psql -h localhost -U postgres -d TasksDatabase
5.Execute docker exec -it my-postgres psql -U postgres
6.Execute postgres=# \l (You will see a list of databases in your postgres)
7.Execute \dt to see the tables in the database in that container
8.select \* from "Tasks"; (if you have migrations executed already)

# Project files

- `Program.cs` - application bootstrap (JWT auth, CORS, Swagger,DBContext)
- `appsettings.json` - configuration (ConnectionStrings, Jwt keys)
- `TaskManagement/` - main server code
  - `Auth/` - authentication models and `AuthService`
  - `Controllers/` - `AuthMgt.cs`, `TaskMgtController.cs`
  - `Data/` - `AppDataContext.cs`
  - `Migrations/` - EF Core migrations (generated)
  - `Models/` - `TasksModel.cs`
  - `Services/` - `TaskManagementService` (task business logic)
  - `UserManagement/Users.cs` - `Users` entity

### Authentication and Authorization

User authentication is done using the login endpoint.
When a user is authenticated a JWT token with basic user data,a JWT Token is returned.
Authorization set up has been added to the swagger page. Simply login then click the Authorize icon where you will paste the token.

### Logging

Used Loggers to show the status of the applications when its running.

### Libraries used by the project

- AspNetCore EntityFrameworkCore
- AspNetCore EntityFrameworkCore Designer
- AspNetCore EntityFrameworkCore SqlServer
- AspNetCore EntityFrameworkCore Tools
- AspNetCore Identity EntityFramework
- AspNetCore HealthChecks EntityFrameworkCore
- Microsoft IdentityModel Tokens
- AspNetCore HealthChecks (SQL Server)
- AspNetCore Authentication JWT Bearer
- Newtonsoft Json
- Serilog AspNetCore, Console, File

### Running the project

dotnet run
dotnet run --project TaskManagementAPI

```

Running in watch mode

```

dotnet watch --project TaskManagementAPI

```

### Database setup

dotnet tool install --global dotnet-ef
dotnet ef migrations add Initial --project TaskManagementAPI
dotnet ef database update --project TaskManagementAPI [migration name here]
dotnet ef database update [migration name here] --project TaskManagementAPI
dotnet dev-certs https --trust
Remove a migration

```

dotnet ef migrations remove --project TaskManagementAPI

```

```
