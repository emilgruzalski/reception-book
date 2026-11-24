# ReceptionBook

**ReceptionBook** is a web application for hotel reception staff. It keeps track of rooms, guests and stays, shows what
has to be done today and makes sure that no room is booked twice.

The project was created as part of the engineering thesis
*„Architektura i bezpieczeństwo nowoczesnych aplikacji internetowych na przykładzie aplikacji ReceptionBook”*.

![Room chart with stays as bars and today's departures, arrivals and rooms to clean](docs/screenshots/dashboard.png)

## What the application does

The user interface is in Polish. The screenshots show demo data with fictional guests.

### Room chart

The home page (above) shows every room in the coming days. Stays are bars, so free periods are visible at a glance, and
a bar opens its reservation. The chart moves by a week and shows 14 days, or 7 on a phone.

Next to it is the work for today: departures, arrivals and rooms to clean, with the actions one click away. A guest
cannot be checked in while the room is still occupied or waiting for cleaning.

### Booking a stay

The receptionist picks the dates and the number of guests, and the form offers only rooms that are free in that period
and big enough. The guest is found by name or document number, and the price is calculated from the room rate and the
number of nights. The API checks availability again when the reservation is saved.

![New reservation form with dates, room, guest and calculated price](docs/screenshots/new-reservation.png)

### Reservations

All reservations can be searched by guest or room and filtered by status and period. The available actions depend on the
status of the reservation:

| Action | Reservation status | Room status |
| --- | --- | --- |
| New reservation | Confirmed | unchanged |
| Check-in | Confirmed → Checked in | Available → Occupied |
| Check-out | Checked in → Checked out | Occupied → Cleaning |
| Cancellation | Confirmed → Cancelled | unchanged |

Check-in is possible only during the booked stay and only when the room is ready.

![List of reservations with filters, statuses and actions](docs/screenshots/reservations.png)

### Rooms and guests

Each room has a type, floor, capacity, price and status, e.g. it goes back from cleaning to available when it is ready.
The guest list keeps contact details and document numbers for future stays. Rooms and guests that have reservations
cannot be deleted.

![List of rooms with type, price and status](docs/screenshots/rooms.png)

### Roles

| | Receptionist | Administrator |
| --- | :---: | :---: |
| Room chart, reservations, check-in and check-out | ✓ | ✓ |
| Adding and editing guests, changing the status of a room | ✓ | ✓ |
| Adding, editing and deleting rooms | | ✓ |
| Deleting guests and reservations | | ✓ |
| Employee accounts | | ✓ |

The layout also works on a phone:

<img src="docs/screenshots/mobile.png" alt="Today's departures and arrivals on a phone" width="260">

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
