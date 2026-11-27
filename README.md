# FieldOps

[![Backend CI](https://github.com/AboElhassan-Developer/FieldOps/actions/workflows/backend-ci.yml/badge.svg)](https://github.com/AboElhassan-Developer/FieldOps/actions/workflows/backend-ci.yml)

**Multi-tenant field service management platform** for maintenance and service companies — built with ASP.NET Core and Angular.

> 🚧 **Status:** In active development. The backend is being built first; the Angular frontend follows.

---

## What is FieldOps?

FieldOps lets service companies (HVAC, plumbing, electrical, appliance repair, …) run their daily operations on one shared platform, while each company's data stays fully isolated.

**Core workflow**

```
Customer request → Work order → Schedule & assign technician
  → Technician updates status (en route → in progress → completed)
  → Invoice → Payment
```

**Main users**

| Role | Responsibilities |
|---|---|
| Platform Admin | Manages tenant companies on the platform |
| Tenant Admin | Manages the company: users, roles, services, settings |
| Dispatcher | Creates work orders, schedules and assigns technicians |
| Technician | Views assigned jobs, updates status, records parts and notes |

## MVP scope

- Tenant registration and isolation (shared database, tenant-scoped data)
- Users, roles and authentication (JWT + refresh tokens)
- Customers and service sites
- Service catalog
- Work orders with status workflow and history
- Scheduling and technician assignment with double-booking prevention
- Real-time notifications for technicians
- Invoices and manual payment recording
- Basic dashboard
- Arabic / English UI with RTL support

See [docs/ROADMAP.md](docs/ROADMAP.md) for planned phases and postponed features.

## Tech stack

| Area | Technology |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API |
| Data | SQL Server, Entity Framework Core |
| Architecture | Modular Monolith |
| Auth | ASP.NET Core Identity, JWT, refresh tokens |
| Validation & logging | FluentValidation, Serilog |
| API docs | OpenAPI + Scalar |
| Real-time | SignalR |
| Testing | xUnit, Testcontainers |
| Frontend | Angular (planned) |
| DevOps | Docker, GitHub Actions |

## Documentation

- [Roadmap](docs/ROADMAP.md)
- [MVP user stories](docs/USER_STORIES.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Architecture decisions](docs/DECISIONS.md)
- [Deployment](docs/DEPLOYMENT.md)

## Getting started

Setup instructions will be added once the backend solution is created.

**Prerequisites (planned):** .NET 10 SDK, SQL Server 2022+ (or Docker), Node.js LTS (for the frontend).

---

© 2026 Abo Elhassan Mamdoh. All rights reserved.
