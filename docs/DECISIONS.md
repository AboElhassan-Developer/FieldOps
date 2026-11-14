# Architecture Decision Records

Each record captures a significant decision, its context, and its consequences.
Status values: **Accepted**, **Superseded**, **Deprecated**.

---

## ADR-001: Target .NET 10 (LTS)

**Status:** Accepted

**Context:** .NET 8 and .NET 9 reached end of support in November 2026. .NET 10 is the current Long-Term Support release (supported until November 2028).

**Decision:** Build on .NET 10 and ASP.NET Core 10.

**Consequences:** Long support window and current language/runtime features. Requires the .NET 10 SDK on all development machines and CI.

---

## ADR-002: Modular monolith architecture

**Status:** Accepted

**Context:** The product has several business areas (identity, customers, operations, billing) but a single team and a single deployment target. Microservices would add network, deployment and data-consistency complexity without a matching benefit.

**Decision:** One deployable application split into modules (one project per module), sharing one database with a schema per module. Modules interact only through contracts and domain events.

**Alternatives considered:**
- *Layered Clean Architecture (4 projects):* simpler at first, but business areas tend to blur over time.
- *Microservices:* rejected — no scaling or team-autonomy requirement justifies the operational cost.

**Consequences:** Clear boundaries and an easy path to extract a module later if ever required. Boundaries must be enforced with architecture tests.

---

## ADR-003: Shared-database multi-tenancy with `TenantId`

**Status:** Accepted

**Context:** Many small tenants with identical schema; low operating cost is required.

**Decision:** All tenants share one database. Tenant-owned entities carry a `TenantId`, and EF Core global query filters restrict queries to the current tenant.

**Alternatives considered:**
- *Database per tenant:* strongest isolation, but higher cost and complex migrations.
- *Schema per tenant:* middle ground, still complex to migrate and manage.

**Consequences:** Cheapest and simplest to operate. Data leakage is the main risk, mitigated by global filters, tenant resolution from the authenticated token, and dedicated isolation tests.

---

## ADR-004: SQL Server with Entity Framework Core

**Status:** Accepted

**Decision:** SQL Server as the database; EF Core as the primary data-access technology. Raw SQL or a micro-ORM may be used for reporting queries where EF Core is not a good fit.

**Consequences:** Strong tooling and wide industry use. Hosting options are narrower than for PostgreSQL, which is acceptable.

---

## ADR-005: ASP.NET Core Identity with JWT access tokens and refresh tokens

**Status:** Accepted

**Decision:** ASP.NET Core Identity for user management; short-lived JWT access tokens; refresh tokens stored server-side and delivered to the browser in an HttpOnly, Secure cookie.

**Alternatives considered:** External identity providers (Keycloak, Microsoft Entra ID) — production-grade, but add an extra service to host and operate, and tenant-scoped users and roles are fully covered by the built-in Identity system.

**Consequences:** No additional infrastructure and full control over authentication flows; refresh-token rotation and revocation must be implemented and tested.

---

## ADR-006: No MediatR or AutoMapper

**Status:** Accepted

**Context:** Both libraries moved to commercial licensing in 2025. The project must remain free of paid dependencies.

**Decision:** Use plain command/query handler classes resolved through dependency injection, and explicit mapping code.

**Consequences:** No licensing cost, explicit and debuggable code; slightly more boilerplate.

---

## ADR-007: Controllers for the HTTP API

**Status:** Accepted

**Decision:** Use controller-based endpoints.

**Alternatives considered:** Minimal APIs — lighter, but controllers remain the most common style in existing codebases.

**Consequences:** Familiar structure, built-in conventions for filters and model binding.

---

## ADR-008: Supporting libraries

**Status:** Accepted

| Concern | Choice |
|---|---|
| Validation | FluentValidation |
| Logging | Serilog |
| API documentation | Microsoft.AspNetCore.OpenApi + Scalar |
| Testing | xUnit, Testcontainers |

All are free and open source.

---

## ADR-009: Monorepo

**Status:** Accepted

**Decision:** Backend, frontend, documentation and CI workflows live in a single repository.

**Consequences:** One place to review the whole product; API and UI changes can ship in one pull request. CI uses path filters so backend and frontend jobs run only when relevant files change.

---

## ADR-010: Serve the Angular app from the API host (same origin)

**Status:** Accepted

**Context:** Refresh tokens are stored in cookies. Hosting the frontend and API on different domains requires cross-site cookies and CORS, which browsers increasingly restrict, and a custom domain to work around.

**Decision:** The Angular build output is served as static files by the ASP.NET Core application. UI and API share one origin.

**Consequences:** No CORS configuration, first-party cookies, a single deployment, and no paid domain required. Frontend and backend are deployed together.

---

## ADR-011: Zero-cost infrastructure

**Status:** Accepted

**Decision:** Only free tools and services are used. Production hosting uses a free .NET hosting plan with SQL Server; the final provider is chosen at the first deployment. The application stays host-agnostic: configuration through environment variables and a Dockerfile available.

**Consequences:** Free tiers have resource limits and may sleep when idle (cold starts). Redis is used locally; production caching is in-memory unless a free option is adopted.

---

## ADR-012: Internationalization (Arabic / English) from the start

**Status:** Accepted

**Context:** The target market is Arabic-speaking. Retrofitting i18n and RTL later would require changes to every screen.

**Decision:** The UI supports Arabic and English with runtime switching and RTL layout. The API returns stable error codes and localized messages. Business data is stored as entered and not translated in the MVP.

**Consequences:** Every UI string goes through translation files; styles use logical CSS properties.

---

## ADR-013: API versioning from the first endpoint

**Status:** Accepted

**Decision:** All endpoints are versioned in the URL (`/api/v1/...`).

**Consequences:** Breaking changes can be introduced as a new version without affecting existing clients.
