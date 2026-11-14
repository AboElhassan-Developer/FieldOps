# Deployment

> Deployment plan. The hosting provider is selected at the first deployment (Sprint 3).

## Constraints

- **Zero cost:** only free tools, services and hosting plans.
- **Host-agnostic:** configuration through environment variables; a Dockerfile is maintained so the app can move between hosts.

## Target topology

```
GitHub repository
  └─ GitHub Actions
       ├─ Pull requests: build + tests
       └─ main branch: build → publish → deploy
                                │
                                ▼
                 Free .NET hosting (single app)
                 ├─ ASP.NET Core API  (/api/v1)
                 ├─ Angular static files (same origin)
                 └─ SQL Server database
```

A single deployment serves both the UI and the API, so no custom domain or CORS configuration is required. The host's default subdomain and managed HTTPS are used.

## Environments

| Environment | Where | Secrets |
|---|---|---|
| Development | Developer machine | .NET User Secrets (outside the repository) |
| CI | GitHub Actions; database via Testcontainers | GitHub Actions secrets |
| Production | Free hosting provider | Host environment variables |

**Rule:** no connection strings, keys or passwords are ever committed.

## Database migrations

Migrations are applied as a separate, explicit deployment step — never automatically at application startup in production.

## Operations

| Concern | Approach |
|---|---|
| Logging | Serilog (console locally, host logs in production) |
| Health | `/health` endpoint plus a free uptime monitor |
| Backups | Provider backups where available, plus periodic database export |
| Caching | In-memory in production; Redis in local development |

## Free-tier trade-offs

- The app may sleep when idle; the first request after idle time is slower (cold start).
- Limited CPU, memory and database size — sufficient for a demo, not for production load.
- Provider terms can change; the host-agnostic setup keeps migration cheap.
