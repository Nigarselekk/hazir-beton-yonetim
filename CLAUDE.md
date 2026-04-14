# CLAUDE.md — Hazır Beton Operasyon Yönetim Sistemi

## Project Overview

A centralized operations management system for a ready-mix concrete company.
It covers order intake, manager approval, vehicle assignment, end-of-day delivery recording,
cost analysis, SMS notifications, and reporting — across web, mobile, and desktop clients.

Source of truth: `docs/project-plan.md` (Turkish). All business rules there are final and must be preserved exactly.

---

## Tech Stack

| Layer | Choice |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Database | PostgreSQL |
| ORM | Entity Framework Core 8 (Npgsql provider) |
| Auth | JWT Bearer tokens, role + permission based |
| SMS | TBD (3rd party Turkish provider, e.g. Netgsm) |
| Reports | PDF: QuestPDF or FastReport; Excel: ClosedXML |
| Clients | Web (TBD), Mobile (TBD), Desktop (TBD) |

All business logic lives **exclusively in the API**. Clients are thin consumers.

---

## Architecture Principles

- **API-first**: every rule enforced in the backend, never duplicated per platform.
- **No automatic behaviors**: vehicle assignment, order approval, and day-close are all manual.
- **Soft-delete**: vehicles and personnel are never hard-deleted; use `Pasif` / `SistemDışı` statuses to preserve historical records.
- **Dual time fields**: every order stores both `TalepEdilenSaat` (requested) and `OnaylananRandevuSaati` (approved). Never merge them.
- **Dual quantity fields**: every order stores both `TalepEdilenMiktar` (requested m³) and `TeslimEdilenMiktar` (actual delivered m³). Never merge them.
- **Item-level costs**: cost entries are always per item (`çimento`, `mazot`, `elektrik`, etc.). A total-only field is not acceptable.

---

## Modules

| Module | Turkish Name | Purpose |
|---|---|---|
| Dashboard | Dashboard | Read-only summary screen; contains operational calendar |
| Orders | Program | Core order lifecycle management |
| Fleet | Filo | Vehicle and maintenance management |
| Personnel | Personel | Staff linked to vehicles (not directly to orders) |
| Customers | Müşteriler | Customer + site (şantiye) data |
| Cost | Maliyet | Daily item-level cost entry and profitability analysis |
| Settings | Ayarlar | User, role, and permission management — Ana Yönetici ONLY |

---

## Critical Invariant Business Rules

These are fixed for the entire project. Do NOT change them without explicit user instruction.

1. **Order entry**: Only `Operasyoncu` creates orders — never the customer.
2. **Order approval**: Only `Yönetici` (Ana or Yan) approves orders.
3. **Time separation**: `TalepEdilenSaat` ≠ `OnaylananRandevuSaati`. Always two fields.
4. **Quantity separation**: `TalepEdilenMiktar` ≠ `TeslimEdilenMiktar`. Always two fields.
5. **Manual vehicle assignment**: The system never auto-assigns vehicles.
6. **Manual day-close**: Delivery is only marked `Teslim Edilen` after a human enters the real m³.
7. **Multi-site customers**: One customer can have many sites (şantiye). This relation must be preserved everywhere.
8. **Item-level costs**: Costs must be stored per item per day. Aggregate-only storage is not acceptable.
9. **Dashboard is read-only**: No data creation or approval on the Dashboard.
10. **Calendar is inside Dashboard**: The operational calendar is a view inside Dashboard, not a replacement for the Program module.
11. **User management — Ana Yönetici only**: Creating users, editing users, assigning roles, and granting permissions belongs **exclusively** to Ana Yönetici. This authority cannot be delegated to any other role or permission. This rule is unconditional.
12. **Settings module — Ana Yönetici only**: No other role can access or see the Settings module.
13. **GPS is a future phase**: Core system must be designed to accommodate GPS later, but GPS features are not part of the initial build.
14. **External systems are data sources**: Netsis (customer/commercial data) and Olympos (dispatch, vehicle, concrete type, m³) are read integrations, not write targets.

---

## Order Lifecycle (Fixed Reference)

```
Customer → Site → [Operasyoncu creates order] → Onay Bekleyen
→ [Yönetici reviews, may change time] → Randevu SMS to customer → Onaylanan
→ [Yönetici manually assigns vehicle] → Operasyon
→ [Operasyoncu enters real m³ at day end] → Teslim Edilen → Teslim SMS
→ Day-end report → Cost analysis
```

**Order statuses**: `OnayBekleyen` | `Onaylanan` | `TeslimEdilen` | `İptalEdilen`

---

## User Roles

| Role | Turkish | Key Capability |
|---|---|---|
| Ana Yönetici | Ana Yönetici | Everything, including user management |
| Yan Yönetici | Yan Yönetici | Approve orders, assign vehicles, view reports |
| Operasyoncu | Operasyoncu | Create orders, enter day-end delivery |
| Muhasebe / Finans | Muhasebe | View and enter cost data, reports |
| İzleyici / Santralci | İzleyici | View only — no mutations |

---

## SMS Events

1. **Approval SMS**: sent to the customer contact when an order is approved.
   - Content: approved time, quantity, concrete type.
2. **Delivery SMS**: sent when day-end actual delivery is entered.
   - Content: actual delivered m³, concrete type.

No SMS for maintenance alerts (internal system warning only).

---

## Future Phases (Do Not Implement in Core)

- GPS Phase 1: internal fleet tracking
- GPS Phase 2: customer-facing live tracking
- GPS Phase 3: ETA, live map, movement history
- Customer-side limited portal (order tracking link, mobile view)
- Dashboard integration of external data (Netsis + Olympos combined view)

---

## What NOT to Do

- Do not auto-assign vehicles.
- Do not merge requested and approved times/quantities into a single field.
- Do not allow any role other than Ana Yönetici to manage users or permissions.
- Do not hard-delete vehicles or personnel.
- Do not store only aggregate cost totals.
- Do not put order creation or approval logic on the Dashboard.
- Do not duplicate business logic per platform.
