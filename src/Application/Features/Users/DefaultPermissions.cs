using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Users;

public static class DefaultPermissions
{
    public static List<Permission> For(UserRole role) => role switch
    {
        UserRole.HeadManager => Enum.GetValues<Permission>().ToList(),

        UserRole.SubManager =>
        [
            Permission.CustomersRead,
            Permission.CustomersWrite,
            Permission.FleetRead,
            Permission.FleetWrite,
            Permission.PersonnelRead,
            Permission.PersonnelWrite,
            Permission.OrdersRead,
            Permission.OrdersApprove,
            Permission.OrdersAssignVehicle,
            Permission.OrdersCancel,
            Permission.DashboardView,
            Permission.ReportsView
        ],

        UserRole.Operator =>
        [
            Permission.CustomersRead,
            Permission.CustomersWrite,
            Permission.FleetRead,
            Permission.PersonnelRead,
            Permission.OrdersRead,
            Permission.OrdersCreate,
            Permission.OrdersDeliveryEntry,
            Permission.DashboardView
        ],

        UserRole.Accounting =>
        [
            Permission.CustomersRead,
            Permission.FleetRead,
            Permission.PersonnelRead,
            Permission.OrdersRead,
            Permission.CostRead,
            Permission.CostWrite,
            Permission.DashboardView,
            Permission.ReportsView
        ],

        UserRole.Viewer =>
        [
            Permission.CustomersRead,
            Permission.FleetRead,
            Permission.PersonnelRead,
            Permission.OrdersRead,
            Permission.CostRead,
            Permission.DashboardView
        ],

        _ => []
    };
}
