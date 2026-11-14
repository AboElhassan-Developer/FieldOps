# Roadmap

FieldOps is built backend-first, then the Angular frontend, with deployment introduced early.
Phases are ordered by dependency; durations are not fixed.

## Phase 1 — Foundation

| Sprint | Goal |
|---|---|
| S1 | Requirements, domain model and ERD, solution structure, CI pipeline (build + test) |
| S2 | Identity and multi-tenancy: tenant registration, login, JWT + refresh tokens, tenant isolation, first integration tests |
| S3 | Customers and sites; validation, error handling (ProblemDetails), logging, pagination, request localization; **first deployment** |

## Phase 2 — Core domain

| Sprint | Goal |
|---|---|
| S4 | Service catalog; work orders with status workflow and history |
| S5 | Scheduling and technician assignment, double-booking prevention, real-time notifications (SignalR) |
| S6 | Invoices and payments: transactions, idempotency, outbox |

## Phase 3 — Hardening

| Sprint | Goal |
|---|---|
| S7 | Caching, rate limiting, background jobs, performance review |
| S8 | Continuous deployment, health checks, monitoring, backups, demo data, security review |

## Phase 4 — Frontend (Angular)

| Sprint | Goal |
|---|---|
| S9 | Project setup, authentication flow, layout, i18n (Arabic / English) and RTL |
| S10 | Customers and work orders screens |
| S11 | Dispatch schedule board, real-time updates, technician view |
| S12 | Dashboard, invoices, polish, production deployment |

## Phase 5 — Release readiness

| Sprint | Goal |
|---|---|
| S13 | Demo tenant and sample data, user-facing documentation, architecture diagrams, screenshots |

## Postponed (after MVP)

- Branches per tenant
- Full inventory management (warehouses, transfers, purchase orders)
- Online payment gateway
- Customer self-service portal
- Tenant subscription billing
- Bilingual business data (e.g. service names in Arabic and English)
- Advanced reporting
- Mobile application
- Message broker integration and distributed tracing
