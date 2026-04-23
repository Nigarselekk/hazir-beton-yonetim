using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Users;

public record SetPermissionsRequest(List<Permission> Permissions);
