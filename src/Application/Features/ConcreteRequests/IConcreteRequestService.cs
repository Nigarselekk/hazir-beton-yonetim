using HazirBeton.Application.Common;

namespace HazirBeton.Application.Features.ConcreteRequests;

public interface IConcreteRequestService
{
    Task<PagedResult<ConcreteRequestListDto>> GetListAsync(ConcreteRequestFilterRequest filter);
    Task<ConcreteRequestDto?> GetByIdAsync(Guid id);
    Task<ConcreteRequestDto> CreateAsync(CreateConcreteRequestRequest request, Guid createdByUserId);
    Task<ConcreteRequestDto> ApproveAsync(Guid id, ApproveConcreteRequestRequest request, Guid approvedByUserId);
    Task<ConcreteRequestDto> AssignVehicleAsync(Guid id, AssignVehicleRequest request, Guid assignedByUserId);
    Task RemoveVehicleAsync(Guid id, Guid vehicleId);
    Task<ConcreteRequestDto> DeliverAsync(Guid id, DeliveryEntryRequest request, Guid deliveryRecordedByUserId);
    Task<ConcreteRequestDto> CancelAsync(Guid id, CancelConcreteRequestRequest request, Guid cancelledByUserId);
}
