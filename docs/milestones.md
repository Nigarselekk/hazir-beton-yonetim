# Milestones — Hazır Beton Operasyon Yönetim Sistemi

## Implementation Philosophy

Each milestone produces a **working, testable API layer** before any UI work begins.
Business logic is never implemented twice — it lives only in the ASP.NET Core 8 API.
Milestones are ordered so that every later milestone can build on a stable foundation.

### Naming Convention (enforced throughout all milestones)

| Context | Language | Examples |
|---|---|---|
| Entities, classes, properties | English | `ConcreteRequest`, `RequestedQuantity`, `ApprovedAppointmentDateTime` |
| Enums and enum values | English | `ConcreteRequestStatus.PendingApproval`, `VehicleStatus.UnderMaintenance` |
| Database table and column names | English | `concrete_requests`, `requested_quantity`, `vehicle_status` |
| API endpoints and JSON fields | English | `/api/concrete-requests`, `deliveredQuantity` |
| UI labels and user-facing text | Turkish | "Onay Bekleyen", "Teslim Edilen Miktar" |

Turkish words must never appear in any technical identifier.

---

## Milestone 1 — Solution Structure & Domain Model

**Goal**: A buildable solution with all architectural layers, core domain entities, enums, DbContext, and initial migrations. No business logic enforced yet.

Deliverables:
- Solution structure — 4 projects:
  - `Domain` — entities, enums, value objects (no dependencies)
  - `Application` — interfaces, service contracts, DTOs
  - `Infrastructure` — EF Core, DbContext, repositories, migrations
  - `API` — ASP.NET Core 8 controllers, middleware, startup
- Core domain entities (all names in English):
  - `Customer` — company name, commercial code, phone, address, notes
  - `Site` — name, address; linked to `Customer` (one customer → many sites)
  - `ConcreteRequest` — see field list below
  - `Vehicle` — plate, type, status, maintenance fields
  - `Personnel` — name, phone, role type, active/passive status
  - `VehiclePersonnel` — join entity (vehicle ↔ primary/backup personnel)
  - `User` — username, password hash placeholder (no auth logic yet)
  - `CostEntry` — date, cost item type, quantity, unit, unit price, total, description
  - `SmsLog` — concrete request reference, event type, recipient, content, sent-at timestamp
- `ConcreteRequest` entity fields:
  - `RequestedDateTime` — customer's requested date/time (never merged with approved)
  - `ApprovedAppointmentDateTime` — manager's confirmed time (set on approval)
  - `RequestedQuantity` — customer's requested m³ (never merged with delivered)
  - `DeliveredQuantity` — actual m³ entered at day-end (set on delivery close)
  - `Status` — enum: see `ConcreteRequestStatus` below
  - References: `Customer`, `Site`, `Vehicle` (nullable until assigned), `AssignedBy`, `CreatedBy`
- Core enums (all names in English):
  - `ConcreteRequestStatus`: `PendingApproval` | `Approved` | `Delivered` | `Cancelled`
  - `VehicleStatus`: `Active` | `UnderMaintenance` | `Passive` | `OutOfSystem`
  - `VehicleType`: `ConcreteMixer` | `Pump` | `Excavator` | `SiteVehicle` | `ServiceVehicle`
  - `PersonnelType`: `MixerDriver` | `PumpOperator` | `FieldPersonnel` | `ServiceDriver`
  - `PersonnelAssignmentType`: `Primary` | `Backup`
  - `CostItemType`: `Cement` | `Diesel` | `Electricity` | `Water` | `Sand` | `LaborCost` | `MaintenanceCost` | `Other`
  - `SmsEventType`: `RequestApproved` | `RequestDelivered`
  - `UserRole`: `HeadManager` | `SubManager` | `Operator` | `Accounting` | `Viewer`
- `AppDbContext` with all entity configurations and relationships
- EF Core 8 (Npgsql) initial migration — schema only, no seed data
- Health check endpoint (`GET /health`) — no auth required
- Minimal `User` entity present in schema; no JWT, no role enforcement, no permission system

**Why first**: All later milestones depend on the domain shape, DB schema, and solution structure being stable.

---

## Milestone 2 — Master Data (Customers, Sites, Fleet, Personnel)

**Goal**: All reference entities are manageable via API before any concrete request can be created. Auth is not yet enforced — endpoints are exercised directly for data integrity testing.

Deliverables:
- `Customer` CRUD: company name, commercial code, phone, address, notes
- `Site` CRUD: linked to customer (one customer → many sites); relationship enforced everywhere
- `Vehicle` management: plate, type, `VehicleStatus`, maintenance date fields
- `Personnel` management: name, phone, `PersonnelType`, active/passive
- Vehicle ↔ Personnel relationship: 1 primary + N backup per vehicle (`PersonnelAssignmentType`)
- Soft-delete enforced on vehicles and personnel (status set to `Passive` / `OutOfSystem` — never hard-deleted)
- Maintenance alert logic: flag vehicles whose next maintenance date is within the alert threshold

**Why second**: `ConcreteRequest` references customers, sites, and vehicles. These entities must exist and be stable before the order flow or auth layer is layered on top.

---

## Milestone 3 — Authentication & Authorization

**Goal**: Secure the API with JWT auth, role-based access control, and the user management system. Applied retroactively to all endpoints from Milestones 1 and 2.

Deliverables:
- JWT authentication (login, token refresh)
- Role-based authorization middleware (5 roles from `UserRole` enum)
- Module + operation-level permission system
- User management endpoints — `HeadManager` only:
  - create user, edit user, deactivate user, assign role, manage permissions
- Settings module access guard: only `HeadManager` may access
- All existing endpoints from Milestones 1 and 2 secured with appropriate role/permission guards
- All other endpoints return `403` if role/permission check fails

**Invariant**: The rule that only `HeadManager` manages users and permissions is enforced here and never relaxed.

**Why third**: Domain and master data are fully shaped before security is layered on. Auth must be complete before the core business flow is exposed.

---

## Milestone 4 — Core Request Flow (Program Module)

**Goal**: The full `ConcreteRequest` lifecycle from creation to delivery, with all invariants enforced.

Deliverables:
- `ConcreteRequest` creation by `Operator`:
  - customer, site, requester name, phone fields
  - material / concrete type, `RequestedQuantity` (m³), price, waybill type, delivery method
  - `RequestedDateTime` (customer's requested time)
  - initial status: `PendingApproval`
- `ConcreteRequest` approval by `SubManager` or `HeadManager`:
  - optional time override → stored as `ApprovedAppointmentDateTime` (never overwrites `RequestedDateTime`)
  - status transition: `PendingApproval` → `Approved`
- Manual vehicle assignment by manager (on request detail):
  - shows vehicle status and maintenance warnings
  - linked personnel becomes visible on the concrete request
  - system never auto-assigns a vehicle
- End-of-day delivery entry by `Operator`:
  - enter `DeliveredQuantity` (actual m³, never overwrites `RequestedQuantity`)
  - status transition: `Approved` → `Delivered`
- `ConcreteRequest` cancellation: `Cancelled` status
- Full status transition validation (no state skipping)
- `ConcreteRequest` list with filters: status, date, customer, site

**Why fourth**: Auth is in place. All reference data exists. The full lifecycle can now be built and enforced correctly.

---

## Milestone 5 — SMS Notifications

**Goal**: Automated SMS at the two critical lifecycle events.

Deliverables:
- SMS provider integration (e.g. Netgsm or equivalent Turkish provider)
- SMS abstraction interface (`ISmsProvider`) — provider-swappable
- Approval SMS: sent on `Approved` transition → customer contact
  - content (Turkish UI text): approved time, `RequestedQuantity`, concrete type
- Delivery SMS: sent on `Delivered` transition → customer contact
  - content (Turkish UI text): `DeliveredQuantity`, concrete type
- `SmsLog` record written per send attempt (success or failure)
- Maintenance alerts remain internal only — no SMS

**Why fifth**: Depends on the core request flow being stable.

---

## Milestone 6 — Dashboard

**Goal**: A real-time read-only summary screen that aggregates system state.

Deliverables:
- Daily summary cards: concrete requests today, m³ delivered, pending approvals, unclosed jobs
- Pending approvals list (with link to request detail)
- Today's planned jobs
- Next-day plan view (approved requests for tomorrow)
- Maintenance alert section (vehicles flagged in Milestone 2)
- Operational calendar view:
  - time, customer name (UI: Turkish), site, delivery method, material, quantity, status, assigned vehicle
  - click-through to concrete request detail
- Role-filtered visibility (`Accounting` sees only relevant sections; `Viewer` sees all but cannot act)
- No data creation or mutation on this screen — read-only enforced at API level

**Why sixth**: Depends on concrete requests, vehicles, and personnel existing with real data.

---

## Milestone 7 — Cost Module

**Goal**: Daily item-level cost entry and profitability analysis.

Deliverables:
- Cost entry: date, `CostItemType` enum, quantity, unit, unit price, total, description
- Predefined `CostItemType` values: `Cement`, `Diesel`, `Electricity`, `Water`, `Sand`, `LaborCost`, `MaintenanceCost`, `Other`
- Cost list with date range filter
- Aggregation calculations:
  - Total cost (by day / week / month / year)
  - Total delivered m³ (from concrete request data via `DeliveredQuantity`)
  - Unit cost (total cost / m³)
  - Total revenue (from concrete request prices)
  - Profit / loss
- Top cost item analysis
- Cost distribution view
- Access: `Accounting` and `HeadManager` can enter; `SubManager` can view

**Why seventh**: Depends on delivery data (m³) from the core request flow.

---

## Milestone 8 — Reporting

**Goal**: Exportable daily and periodic reports.

Deliverables:
- End-of-day report:
  - jobs completed, total m³ delivered, per-customer breakdown, planned vs. actual (`RequestedQuantity` vs. `DeliveredQuantity`), unclosed concrete requests
- Next-day plan report:
  - approved concrete requests for tomorrow, time-based schedule, customer/site distribution
- PDF export (QuestPDF or equivalent)
- Excel export (ClosedXML)
- Report access: `HeadManager`, `SubManager`, `Accounting` (within their scope)

**Why eighth**: Needs stable concrete request and cost data to aggregate.

---

## Milestone 9 — External Integrations

**Goal**: Pull data from Netsis and Olympos into the system.

Deliverables:
- Integration abstraction layer (`IExternalDataProvider` interface — not hard-coded per provider)
- **Netsis** connector:
  - import / sync customer data (company name, commercial code)
  - optionally read waybill / commercial data
  - access method: NetOpenX API, direct export, or file import (whichever is feasible)
- **Olympos** connector:
  - import dispatch records: vehicle, plate, driver, concrete type, m³, waybill
  - access method: API or export/import file
- Conflict resolution: imported data vs. manually entered data
- Integration run log: timestamp, record count, errors
- Dashboard extension (Milestone 6): combined view of Netsis + Olympos data alongside internal data

**Note**: Netsis and Olympos are **read-only data sources**. The system never writes back to them.

**Why last in core**: Does not block any internal workflow. Can be developed in parallel but delivered last.

---

## Future Phases (Post-Core)

### Phase GPS-1 — Internal Fleet Tracking
- Real-time vehicle location for internal operators
- Status: in transit / on site / returning

### Phase GPS-2 — Customer-Facing Delivery Tracking
- Per-order tracking link
- Vehicle proximity notifications
- Live map view

### Phase GPS-3 — Advanced Delivery Experience
- ETA calculation
- Vehicle movement history

### Phase Customer Portal
- Customer reads their own concrete requests (no mutations)
- Mobile-friendly limited view
- Delivery tracking linked to GPS phases

---

## Summary Table

| # | Milestone | Depends On |
|---|---|---|
| 1 | Solution Structure & Domain Model | — |
| 2 | Master Data (Customers, Fleet, Personnel) | 1 |
| 3 | Authentication & Authorization | 1, 2 |
| 4 | Core Request Flow (Program Module) | 1, 2, 3 |
| 5 | SMS Notifications | 4 |
| 6 | Dashboard | 4 |
| 7 | Cost Module | 4 |
| 8 | Reporting | 4, 7 |
| 9 | External Integrations (Netsis, Olympos) | 1, 2, 3, 4 |
