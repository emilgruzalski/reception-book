# ReceptionBook – Frontend

Angular client for **ReceptionBook**, a web application for hotel reception staff.

The project was created as part of the engineering thesis
*„Architektura i bezpieczeństwo nowoczesnych aplikacji internetowych na przykładzie aplikacji ReceptionBook”*.

The REST API lives in the [`backend`](../backend) directory.

## Features

- Dashboard with today's arrivals and departures, room status and occupancy
- Rooms, guests and reservations with server-side paging, sorting, filtering and search
- Reservation form that shows only rooms available in the selected period
- Check-in, check-out and cancellation of reservations
- User management for administrators and password change for every user
- Polish user interface (dates, currency, paginator)

## Security

- JWT access token sent only to the application API by an HTTP interceptor
- Automatic token refresh after `401 Unauthorized`; concurrent requests wait for a single refresh
- Refresh token revoked on the server when the user logs out
- Route guards (`authGuard`, `roleGuard`) and the `*appHasRole` directive – the API enforces the same rules
- Open redirect protection of the `returnUrl` after login
- Angular template sanitization – no `innerHTML` and no `bypassSecurityTrust*` calls
- nginx with `Content-Security-Policy` (`script-src 'self'`), `X-Frame-Options`, `X-Content-Type-Options` and `Referrer-Policy`

## Tech stack

Angular 16, Angular Material 16, RxJS, `@auth0/angular-jwt`, Jasmine and Karma, nginx, Docker.

## Getting started

Requirements: Node.js 18 and a running [backend](../backend).

```bash
npm ci
npm start
```

The application is available at <http://localhost:4200>. In development it calls the API at
`https://localhost:7285/api` (see `src/environments/environment.development.ts`).

## Tests

```bash
npm test          # watch mode in Chrome
npm run test:ci   # single run in headless Chrome
```

## Docker

The production build is served by nginx, which also forwards `/api` requests to the backend:

```bash
docker build -t receptionbook-web .
docker run -p 4200:80 -e API_URL=http://host.docker.internal:8080 receptionbook-web
```

## Project structure

```
src/app
├── core        # models, services, interceptors, guards and utilities
├── shared      # Material modules, pipes, directives, validators and common components
├── layout      # main layout with toolbar and navigation
└── features    # lazy-loaded modules: dashboard, rooms, guests, reservations, users, account, auth
```
