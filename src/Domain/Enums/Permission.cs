namespace HazirBeton.Domain.Enums;

public enum Permission
{
    // Customers & Sites module
    CustomersRead,
    CustomersWrite,

    // Fleet module
    FleetRead,
    FleetWrite,

    // Personnel module
    PersonnelRead,
    PersonnelWrite,

    // Orders module (Milestone 4)
    OrdersRead,
    OrdersCreate,
    OrdersApprove,
    OrdersAssignVehicle,
    OrdersDeliveryEntry,
    OrdersCancel,

    // Cost module
    CostRead,
    CostWrite,

    // Dashboard
    DashboardView,

    // Reports
    ReportsView
}
