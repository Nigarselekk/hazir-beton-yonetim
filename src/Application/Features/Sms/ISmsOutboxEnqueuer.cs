using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Sms;

public interface ISmsOutboxEnqueuer
{
    // Adds a SmsLog row in the current DbContext. Does NOT call SaveChanges —
    // the caller's transaction owns the commit so business writes and the outbox
    // row land atomically.
    Task EnqueueAsync(ConcreteRequest request, SmsEventType eventType, CancellationToken cancellationToken = default);
}
