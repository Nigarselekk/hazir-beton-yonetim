namespace HazirBeton.Application.Features.ConcreteRequests;

public record ApproveConcreteRequestRequest(
    DateTime? ApprovedAppointmentDateTime,
    uint RowVersion
);
