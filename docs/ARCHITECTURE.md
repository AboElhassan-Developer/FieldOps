# Architecture

> This document describes the target architecture. Details are refined as each sprint is implemented.

## Overview

FieldOps is a **modular monolith**: one deployable ASP.NET Core application, internally split into modules with explicit boundaries.

```
                ┌──────────────────────────────────────────┐
  Browser ────► │  FieldOps.Api (host)                     │
  (Angular,     │   ├─ serves Angular static files         │
   same origin) │   ├─ /api/v1/...  REST endpoints         │
                │   └─ /hubs/...    SignalR                │
                │                                          │
                │  Modules                                 │
                │   ├─ Identity    tenants, users, roles   │
                │   ├─ Customers   customers, sites        │
                │   ├─ Operations  services, work orders,  │
                │   │              scheduling              │
                │   └─ Billing     invoices, payments      │
                │                                          │
                │  SharedKernel   base types, results,     │
                │                 domain event abstractions│
                └───────────────────┬──────────────────────┘
                                    │
                             SQL Server (one database,
                             one schema per module)
```

## Planned solution layout

```
backend/
├── src/
│   ├── FieldOps.Api/
│   ├── FieldOps.SharedKernel/
│   └── Modules/
│       ├── FieldOps.Modules.Identity/
│       ├── FieldOps.Modules.Customers/
│       ├── FieldOps.Modules.Operations/
│       └── FieldOps.Modules.Billing/
└── tests/
    ├── FieldOps.UnitTests/
    ├── FieldOps.IntegrationTests/
    └── FieldOps.ArchitectureTests/
frontend/   (Angular, planned)
```

Each module is a single project organized internally by folders (Domain, Application, Infrastructure, Endpoints).

## Key principles

- **Module boundaries:** modules communicate through public contracts and domain events only; architecture tests enforce this.
- **Multi-tenancy:** shared database; every tenant-owned row has a `TenantId`; EF Core global query filters scope all queries to the current tenant resolved from the authenticated user.
- **Extensibility:** postponed features (inventory, branches, payment providers, notification channels) are added as new modules or implementations without rewriting existing ones.
- **API:** RESTful, versioned from the start (`/api/v1`), errors returned as RFC 9457 ProblemDetails with stable error codes.
- **Localization:** UI text and API error messages support Arabic and English; business data is stored as entered.
- **Time:** all timestamps stored in UTC (`DateTimeOffset`).

See [DECISIONS.md](DECISIONS.md) for the reasoning behind each choice.
