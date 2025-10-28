# TaskManagementAPI (FullStack-v1-TaskManagementSystem)

Lightweight Task Management API built with .NET 8, Entity Framework Core and PostgreSQL. The API provides user registration/login (JWT-based) and basic task CRUD. It was created as part of a Full Stack project skeleton.

This README covers: project structure, configuration, how to build/run locally, database migrations, exposed endpoints (auth + tasks), sample requests, and troubleshooting notes.

---

## Repo layout (important files)

- `Program.cs` - application bootstrap (JWT auth, CORS, Swagger)
- `appsettings.json` - configuration (ConnectionStrings, Jwt keys)
- `TaskManagement/` - main server code
  - `Auth/` - authentication models and `AuthService`
  - `Controllers/` - `AuthMgt.cs`, `TaskMgtController.cs`
  - `Data/` - `AppDataContext.cs`
  - `Migrations/` - EF Core migrations (generated)
  - `Models/` - `TasksModel.cs`
  - `Services/` - `TaskManagementService` (task business logic)
  - `UserManagement/Users.cs` - `Users` entity

---

## Quick start (dev)

Prereqs:

- .NET 8 SDK
- PostgreSQL (or update `DefaultConnection` to another provider)
- Optional: `dotnet-ef` global tool if running migrations locally: `dotnet tool install --global dotnet-ef`

1. Restore & build

```bash
dotnet restore
dotnet build
```

2. Configure database

- Edit `appsettings.json` → `ConnectionStrings:DefaultConnection` to point to your Postgres instance.

3. (Optional) Apply EF migrations

```bash
# from repo root
dotnet ef database update --project ./TaskManagementAPI.csproj
```

4. Run

By default the app listens on URLs configured in `launchSettings.json` (commonly https://localhost:7290). If that port is in use, run with an alternate URL:

```bash
# Run on http:5000 and https:5001 (example)
dotnet run --urls "http://localhost:5000;https://localhost:5001"
```

Swagger UI (when `ASPNETCORE_ENVIRONMENT=Development`) is available at `/swagger/index.html` (e.g. `http://localhost:5000/swagger/index.html`).

---

## Configuration notes

- JWT keys and issuer/audience are in `appsettings.json` under the `Jwt` section. The app reads `Jwt:Key` for signing tokens.
- Connection string lives in `ConnectionStrings:DefaultConnection`.
- Dev HTTPS certificate warning is normal. To trust the dev cert run:

```bash
dotnet dev-certs https --trust
```

---

## Important implementation details / caveats

- Passwords are hashed using BCrypt (package `BCrypt.Net-Next`).
- The `Users` entity in code uses the property `PasswordHash`, but the database column (created by the initial migration) is named `Password`. The code maps `PasswordHash` to the existing `Password` column to avoid immediate schema changes.
- The `Tasks` table stores `UserId` as text (string) — GUIDs are saved as strings. Service methods convert Guid to string when querying.

If you want the DB to use `PasswordHash` as the column or to change `UserId` to a proper `uuid` column, generate and apply a migration to update the schema.

---

## Endpoints

All endpoints are prefixed by `/api` (based on controller routes).

### Auth (authentication)

Controller: `AuthManagementController` → route `api/AuthManagement`

- POST `/api/AuthManagement/register`

  - Request JSON:
    ```json
    {
      "username": "alice",
      "password": "Pass@123",
      "firstName": "Alice",
      "lastName": "Smith"
    }
    ```
  - Response: 200 OK with created user summary (`userId`, `username`) or 400 if username exists.

- POST `/api/AuthManagement/login`

  - Request JSON:
    ```json
    { "username": "alice", "password": "Pass@123" }
    ```
  - Response: 200 OK `{ "token": "<jwt>" }` or 401 Unauthorized on invalid credentials.

- GET `/api/AuthManagement/{id}`
  - Path parameter: `id` is a GUID (user id).
  - Response: 200 OK with user details `{ userId, username, firstName, lastName }` or 404 if not found.

Notes

- JWT generation uses `Jwt:Key` from configuration. Ensure this key is set in `appsettings.json` or secrets for token issuance.

### Tasks

Controller: `TaskManagementController` → route `api/TaskManagement`

- GET `/api/TaskManagement/list-of-tasks`

  - Returns all tasks.

- GET `/api/TaskManagement/get-task-by-id?id={taskId}` (or route param variant)

  - `taskId` as Guid string. Returns task or 404.

- POST `/api/TaskManagement/create-task`

  - Payload example (assign to user by `userId` string GUID):
    ```json
    {
      "taskName": "Fix bug",
      "description": "Fix authentication bug",
      "status": 0,
      "userId": "1683c999-8433-4c62-8545-b09233657dd4"
    }
    ```
  - Returns `201 Created` with created task.

- PUT `/api/TaskManagement/update-task`

  - Accepts `UpdateTaskModel` (contains `taskId` and fields to update). Returns updated view model.

- DELETE `/api/TaskManagement/delete-task/{id}`

  - Deletes a task by id.

- GET `/api/TaskManagement/user/{userId}`
  - Returns tasks assigned to the specified user GUID. The controller validates `userId` is a valid GUID string. Example:

```bash
curl -s http://localhost:5000/api/TaskManagement/user/1683c999-8433-4c62-8545-b09233657dd4 | jq
```

Response: 200 OK with an array (may be empty) of tasks for that user.

---

## Sample flows

1. Register user

```bash
curl -X POST http://localhost:5000/api/AuthManagement/register \
  -H "Content-Type: application/json" \
  -d '{"username":"alice","password":"Pass@123","firstName":"Alice","lastName":"Smith"}'
```

2. Login to get token

```bash
curl -X POST http://localhost:5000/api/AuthManagement/login \
  -H "Content-Type: application/json" \
  -d '{"username":"alice","password":"Pass@123"}'
# returns { "token": "..." }
```

3. Create task assigned to that user (use userId from registration response)

```bash
curl -X POST http://localhost:5000/api/TaskManagement/create-task \
  -H "Content-Type: application/json" \
  -d '{"taskName":"My Task","description":"...","status":0,"userId":"<user-guid>"}'
```

4. Get tasks for user

```bash
curl http://localhost:5000/api/TaskManagement/user/<user-guid>
```

---

## Running tests / validation

There are no automated tests included by default. To manually validate:

1. Start the application (see `dotnet run` above).
2. Use Swagger UI, curl, or Postman to exercise the endpoints.

---

## Troubleshooting

- "Address already in use" when starting: change ports with `--urls` or stop the process occupying the port (e.g. `sudo ss -ltnp | grep :7290` then `kill <PID>`).
- Dev certificate warnings: run `dotnet dev-certs https --trust`.
- EF/DB column mismatch (e.g., `PasswordHash` vs `Password`): the project currently maps the `PasswordHash` property to the DB column `Password`. If you rename columns in code, generate and apply an EF migration.

---

## Next improvements (suggestions)

- Convert `Tasks.UserId` DB column to `uuid` and model to `Guid` (migration required).
- Rename DB column `Password` → `PasswordHash` via EF migration to avoid attribute mapping.
- Add JWT-protected endpoints for task operations (require Authorization header) and role-based access control.
- Add unit/integration tests.

---

If you'd like, I can also:

- generate the EF migration to rename `Password` to `PasswordHash`,
- generate migration to convert `Tasks.UserId` from `text` to `uuid` and migrate existing rows,
- add example Postman collection or HTTPie scripts.

Happy to proceed with any of the above — tell me which you'd like next.
