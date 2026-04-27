using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Sms;

public interface ISmsContentBuilder
{
    string Build(ConcreteRequest request, SmsEventType eventType);
}
