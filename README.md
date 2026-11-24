# ReceptionBook

**ReceptionBook** is a web application for hotel reception staff: rooms, guests, reservations,
check-in / check-out and employee accounts.

The project was created as part of the engineering thesis
*„Architektura i bezpieczeństwo nowoczesnych aplikacji internetowych na przykładzie aplikacji ReceptionBook”*.

## Repository structure

| Directory | Description | Tech stack |
| --- | --- | --- |
| [`backend`](backend) | REST API | .NET 8, ASP.NET Core, Entity Framework Core, SQL Server |
| [`frontend`](frontend) | Web client | Angular 16, Angular Material, nginx |

Each directory has its own README with features, security notes and detailed setup instructions.

## Development

Configure the database and secrets as described in the [backend README](backend/README.md#getting-started), then run
both applications in separate terminals:

```bash
cd backend/ReceptionBook
dotnet run --launch-profile https
```

```bash
cd frontend
npm ci
npm start
```

The application is available at <http://localhost:4200> and the API at <https://localhost:7285/swagger>.

## Docker

Start the database and the API:

```bash
cd backend
cp .env.example .env   # fill in DB_PASSWORD, JWT_SECRET_KEY and ADMIN_PASSWORD
docker compose up --build
```

Then start the client in another terminal:

```bash
cd frontend
docker build -t receptionbook-web .
docker run -p 4200:80 -e API_URL=http://host.docker.internal:8080 receptionbook-web
```

## Tests

```bash
(cd backend && dotnet test)
(cd frontend && npm ci && npm run test:ci)
```
