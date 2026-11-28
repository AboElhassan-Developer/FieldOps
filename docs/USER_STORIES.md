# MVP User Stories

**Priority:** **M** = Must (MVP) · **S** = Should (MVP if time allows)
**Roles:** Platform Admin · Tenant Admin · Dispatcher · Technician

All stories implicitly require that users only ever see and change data belonging to their own tenant (see [NFR-01](#non-functional-requirements)).

---

## E1 — Tenant onboarding & identity

### US-01 Register a company (M)
**As a** company owner, **I want** to register my company on FieldOps, **so that** my team can start managing operations.

- Registration creates a tenant and its first user with the Tenant Admin role, in a single transaction.
- Company name and admin email are required; the email must be unique across the platform.
- Password must meet the password policy (min. 8 characters, upper, lower, digit).
- On success, the admin can sign in immediately.
- Registration is rate-limited per client IP to prevent abuse.
- Email confirmation is postponed (see [Roadmap](ROADMAP.md#postponed-after-mvp)).

### US-02 Sign in (M)
**As a** user, **I want** to sign in with email and password, **so that** I can access my company's workspace.

- Valid credentials return a short-lived access token and set a refresh token in an HttpOnly, Secure cookie.
- Invalid credentials return a generic error (no hint whether the email exists).
- Repeated failures temporarily lock the account.
- Users of a suspended tenant or deactivated users cannot sign in.

### US-03 Stay signed in / sign out (M)
**As a** user, **I want** my session to renew automatically and to be able to sign out, **so that** I'm not interrupted and my account stays secure.

- An expired access token can be renewed with a valid refresh token; each refresh rotates the refresh token.
- Reusing an already-rotated refresh token revokes the whole token family.
- Signing out revokes the current refresh token and clears the cookie.

### US-04 Manage team members (M)
**As a** Tenant Admin, **I want** to add users and assign roles, **so that** each team member has the right access.

- Admin can create a user with name, email, phone, role (Tenant Admin, Dispatcher, Technician) and a temporary password.
- The user must change the temporary password on first sign-in.
- Admin can change a user's role and deactivate / reactivate users.
- Admin can reset a team member's password to a new temporary password (self-service reset by email is postponed).
- Deactivated users cannot sign in; their history is kept.
- An admin cannot deactivate themselves or remove the last active Tenant Admin.

### US-05 Change my password (M)
**As a** user, **I want** to change my password, **so that** I can keep my account secure.

- Requires the current password; the new password must meet the policy.
- All other sessions of the user are revoked.

### US-06 Choose my language (M)
**As a** user, **I want** to choose Arabic or English, **so that** I can work in my preferred language.

- The preference is stored on the user profile and applied on next sign-in.
- API error messages are returned in the user's language.

---

## E2 — Platform administration

### US-07 View tenants (S)
**As a** Platform Admin, **I want** to see all registered companies, **so that** I can monitor platform usage.

- Paged list with name, registration date, status and user count.

### US-08 Suspend / reactivate a tenant (S)
**As a** Platform Admin, **I want** to suspend a company, **so that** I can block access when needed.

- Suspended tenants' users cannot sign in; existing sessions stop working on next refresh.
- Data is retained.

---

## E3 — Customers & sites

### US-09 Manage customers (M)
**As a** Dispatcher, **I want** to add and edit customers, **so that** I can record who requests services.

- Customer types: Individual or Business.
- Name and at least one phone number are required; email is optional.
- Customers with work orders cannot be deleted — only deactivated.

### US-10 Manage customer sites (M)
**As a** Dispatcher, **I want** to record one or more service locations for a customer, **so that** technicians know where to go.

- A site has a label, address, area/city and optional notes (e.g. access instructions).
- A customer can have multiple sites; one can be marked as default.

### US-11 Search customers (M)
**As a** Dispatcher, **I want** to search customers by name or phone, **so that** I can quickly find an existing customer during a call.

- Results are paged and sorted by relevance/name.

---

## E4 — Catalog

### US-12 Manage services (M)
**As a** Tenant Admin, **I want** to define the services my company offers, **so that** work orders and invoices use consistent names and prices.

- A service has a name, category, default price and estimated duration.
- Services can be deactivated; deactivated services can't be used on new work orders.

### US-13 Manage parts (M)
**As a** Tenant Admin, **I want** to maintain a list of spare parts with prices, **so that** technicians can record parts used.

- A part has a name, SKU (unique per tenant), unit and unit price.
- No stock tracking in the MVP.

---

## E5 — Work orders

### US-14 Create a work order (M)
**As a** Dispatcher, **I want** to create a work order for a customer request, **so that** the job is tracked from start to finish.

- Requires customer, site, service, problem description and priority (Low, Medium, High).
- Gets a sequential number per tenant (e.g. `WO-1042`).
- Due date defaults from priority and can be overridden.
- Initial status is **New**.

### US-15 Work order lifecycle (M)
**As a** Dispatcher, **I want** work orders to follow a clear status flow, **so that** everyone knows where each job stands.

| From | Allowed to |
|---|---|
| New | Scheduled, Cancelled |
| Scheduled | En route, New (unassigned), Cancelled |
| En route | In progress |
| In progress | Completed |
| Completed | — |
| Cancelled | — |

- Any other transition is rejected with a clear error.
- Cancelling requires a reason.

### US-16 Status history (M)
**As a** Dispatcher, **I want** to see who changed a work order and when, **so that** I can follow up and resolve disputes.

- Every status change is recorded with timestamp, user and optional note.
- History is read-only.

### US-17 List and filter work orders (M)
**As a** Dispatcher, **I want** to filter work orders by status, technician, priority and date, **so that** I can focus on what needs attention.

- Paged list; overdue work orders (past due date and not completed/cancelled) are flagged.

### US-18 Edit a work order (M)
**As a** Dispatcher, **I want** to edit work order details, **so that** I can correct or update information.

- Editable while not Completed or Cancelled.
- If two users edit the same work order at the same time, the second save is rejected and the user is asked to reload (no silent overwrite).

---

## E6 — Scheduling & assignment

### US-19 Technician profile (M)
**As a** Tenant Admin, **I want** to record technicians' skills (service categories), **so that** dispatchers assign suitable people.

### US-20 Assign and schedule (M)
**As a** Dispatcher, **I want** to assign a technician and a time window to a work order, **so that** the job is planned.

- A work order has exactly one assigned technician in the MVP (assignments are stored separately so multi-technician jobs can be added later).
- The technician must be active and have the Technician role.
- A technician cannot have overlapping time windows — the request is rejected, naming the conflicting work order.
- The rule holds even when two dispatchers assign at the same moment.
- Assigning moves the work order to **Scheduled**.
- A warning (not an error) is shown if the technician lacks the matching skill.

### US-21 Reschedule or unassign (M)
**As a** Dispatcher, **I want** to change the time or technician, **so that** I can handle changes.

- Same conflict rules as US-20.
- Unassigning returns the work order to **New**.

### US-22 Daily schedule view (M)
**As a** Dispatcher, **I want** to see each technician's jobs for a day, **so that** I can spot free slots.

---

## E7 — Technician

### US-23 My jobs (M)
**As a** Technician, **I want** to see my jobs for today and upcoming days, **so that** I know where to go.

- Technicians see only work orders assigned to them.
- For each assigned job, the technician sees the price summary (service, parts used, total) as read-only, so they can answer the customer on site.
- Technicians cannot see invoices, payments, revenue figures or other technicians' jobs.

### US-24 Update job status (M)
**As a** Technician, **I want** to mark a job as en route, in progress and completed, **so that** the office knows my progress.

- Only the assigned technician can change status this way.
- Completing requires a work summary.

### US-25 Record parts used and notes (M)
**As a** Technician, **I want** to record parts used and notes, **so that** the invoice is accurate.

- Parts come from the parts catalog with a quantity; price is captured at the time of recording.
- Editable until the work order is completed.

### US-26 Real-time notifications (M)
**As a** Technician, **I want** to be notified immediately when a job is assigned, rescheduled or cancelled, **so that** I don't miss changes.
**As a** Dispatcher, **I want** to see status changes live, **so that** I don't need to refresh.

---

## E8 — Billing

### US-27 Create an invoice from a work order (M)
**As a** Dispatcher, **I want** to generate an invoice from a completed work order, **so that** I can bill the customer.

- Lines are pre-filled from the service and parts used; lines can be adjusted while the invoice is a **Draft**.
- One invoice per work order; each invoice line references its source work order.
- Tax rate comes from tenant settings.

### US-28 Issue an invoice (M)
**As a** Dispatcher, **I want** to issue an invoice, **so that** it becomes official.

- Issuing assigns a sequential number per tenant (e.g. `INV-2026-0318`) with no gaps or duplicates.
- Issued invoices cannot be edited — only voided with a reason.
- An invoice with recorded payments cannot be voided (refunds are postponed).

### US-29 Record payments (M)
**As a** Dispatcher, **I want** to record payments, **so that** I know what is still owed.

- Payment has amount, date, method (Cash, Bank transfer, Card, Other) and optional reference.
- Partial payments allowed; total paid cannot exceed the invoice total.
- Invoice status updates automatically: Issued → Partially paid → Paid.
- Submitting the same payment twice (e.g. double click, network retry) records it only once.

### US-30 Tenant billing settings (M)
**As a** Tenant Admin, **I want** to set my company's tax rate, currency and invoice details, **so that** invoices are correct.

- One tax rate per tenant, applied to all invoice lines.
- The tax rate is stored on each invoice line when the invoice is issued; changing the setting later never alters issued invoices.

---

## E9 — Dashboard

### US-31 Operations dashboard (S)
**As a** Tenant Admin or Dispatcher, **I want** key numbers at a glance, **so that** I can manage the day.

- Open work orders, scheduled today, overdue, completed this week, revenue this month, unpaid invoices.

---

## Role permissions

| Capability | Platform Admin | Tenant Admin | Dispatcher | Technician |
|---|---|---|---|---|
| Manage tenants | ✅ | — | — | — |
| Manage users & roles | — | ✅ | — | — |
| Tenant settings (tax, currency, invoice details) | — | ✅ | — | — |
| Manage services & parts | — | ✅ | View | View names |
| Manage customers & sites | — | ✅ | ✅ | View (assigned jobs only) |
| Create / edit / cancel work orders | — | ✅ | ✅ | — |
| Assign & schedule technicians | — | ✅ | ✅ | — |
| Update job status, parts used, notes | — | — | — | ✅ (assigned jobs only) |
| View job price summary | — | ✅ | ✅ | ✅ (assigned jobs only) |
| Invoices & payments | — | ✅ | ✅ | — |
| Dashboard | — | ✅ | ✅ | — |

**Postponed:** technicians collecting payment on site and recording it in the app.

---

## Non-functional requirements

| ID | Requirement |
|---|---|
| NFR-01 | **Tenant isolation:** no API call may read or modify another tenant's data; covered by automated tests. |
| NFR-02 | **Authorization:** every endpoint enforces role-based policies. |
| NFR-03 | **Security:** passwords hashed by ASP.NET Core Identity; HTTPS only; secrets never stored in source control. |
| NFR-04 | **Validation & errors:** invalid input returns `400` with field-level errors; all errors use ProblemDetails with a stable error code. |
| NFR-05 | **Localization:** UI and error messages in Arabic and English; RTL layout for Arabic. |
| NFR-06 | **Time:** timestamps stored in UTC and displayed in the tenant's time zone. |
| NFR-07 | **Money:** amounts stored as `decimal` with currency per tenant; no floating-point arithmetic. |
| NFR-08 | **Lists:** all list endpoints are paged. |
| NFR-09 | **Auditability:** creation and last-modification user and time recorded on business entities. |
| NFR-10 | **Observability:** structured logs with correlation IDs; health check endpoint. |
