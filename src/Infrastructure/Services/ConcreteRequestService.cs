using HazirBeton.Application.Common;
using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.ConcreteRequests;
using HazirBeton.Application.Features.Sms;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using HazirBeton.Infrastructure.Services.Sms;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services;

public class ConcreteRequestService : IConcreteRequestService
{
    private const int MaintenanceAlertDays = 7;

    private readonly AppDbContext _context;
    private readonly ISmsOutboxEnqueuer _smsOutbox;

    public ConcreteRequestService(AppDbContext context, ISmsOutboxEnqueuer smsOutbox)
    {
        _context = context;
        _smsOutbox = smsOutbox;
    }

    public async Task<PagedResult<ConcreteRequestListDto>> GetListAsync(ConcreteRequestFilterRequest filter)
    {
        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = filter.PageSize is < 1 or > 100 ? 20 : filter.PageSize;

        var query = _context.ConcreteRequests
            .Include(cr => cr.Customer)
            .Include(cr => cr.Site)
            .Include(cr => cr.ConcreteRequestVehicles)
                .ThenInclude(crv => crv.Vehicle)
            .AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(cr => cr.Status == filter.Status.Value);

        if (filter.FromDate.HasValue)
            query = query.Where(cr => cr.RequestedDateTime >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
        {
            var toDateExclusive = filter.ToDate.Value.Date.AddDays(1);
            query = query.Where(cr => cr.RequestedDateTime < toDateExclusive);
        }

        if (filter.CustomerId.HasValue)
            query = query.Where(cr => cr.CustomerId == filter.CustomerId.Value);

        if (filter.SiteId.HasValue)
            query = query.Where(cr => cr.SiteId == filter.SiteId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(cr => cr.RequestedDateTime)
            .ThenByDescending(cr => cr.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(ToListDto).ToList();

        return new PagedResult<ConcreteRequestListDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<ConcreteRequestDto?> GetByIdAsync(Guid id)
    {
        var entity = await LoadFullAsync(id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<ConcreteRequestDto> CreateAsync(CreateConcreteRequestRequest request, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(request.RequesterName))
            throw new ArgumentException("Talep eden kişi adı zorunludur.");
        if (string.IsNullOrWhiteSpace(request.CompanyContactPhone))
            throw new ArgumentException("Firma iletişim telefonu zorunludur.");
        if (string.IsNullOrWhiteSpace(request.SiteContactPhone))
            throw new ArgumentException("Şantiye iletişim telefonu zorunludur.");
        if (string.IsNullOrWhiteSpace(request.MaterialType))
            throw new ArgumentException("Malzeme türü zorunludur.");
        if (string.IsNullOrWhiteSpace(request.WaybillType))
            throw new ArgumentException("İrsaliye türü zorunludur.");
        if (string.IsNullOrWhiteSpace(request.DeliveryMethod))
            throw new ArgumentException("Teslimat yöntemi zorunludur.");
        if (request.RequestedQuantity <= 0)
            throw new ArgumentException("Talep edilen miktar sıfırdan büyük olmalıdır.");
        if (request.UnitPrice < 0)
            throw new ArgumentException("Birim fiyat negatif olamaz.");
        if (request.RequestedDateTime < DateTime.UtcNow)
            throw new ArgumentException("Geçmiş bir tarih için beton talebi oluşturulamaz.");

        var customerExists = await _context.Customers.AnyAsync(c => c.Id == request.CustomerId);
        if (!customerExists)
            throw new ArgumentException($"'{request.CustomerId}' ID'li müşteri bulunamadı.");

        var site = await _context.Sites.FirstOrDefaultAsync(s => s.Id == request.SiteId);
        if (site is null)
            throw new ArgumentException($"'{request.SiteId}' ID'li şantiye bulunamadı.");

        if (site.CustomerId != request.CustomerId)
            throw new ArgumentException("Seçilen şantiye belirtilen müşteriye ait değil.");

        var entity = new ConcreteRequest
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            SiteId = request.SiteId,
            RequesterName = request.RequesterName.Trim(),
            CompanyContactPhone = request.CompanyContactPhone.Trim(),
            SiteContactPhone = request.SiteContactPhone.Trim(),
            MaterialType = request.MaterialType.Trim(),
            RequestedQuantity = request.RequestedQuantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = request.UnitPrice * request.RequestedQuantity,
            WaybillType = request.WaybillType.Trim(),
            DeliveryMethod = request.DeliveryMethod.Trim(),
            RequestedDateTime = request.RequestedDateTime,
            ApprovedAppointmentDateTime = null,
            Status = ConcreteRequestStatus.PendingApproval,
            Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
            CreatedById = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ConcreteRequests.Add(entity);
        await _context.SaveChangesAsync();

        var full = await LoadFullAsync(entity.Id);
        return ToDto(full!);
    }

    public async Task<ConcreteRequestDto> ApproveAsync(Guid id, ApproveConcreteRequestRequest request, Guid approvedByUserId)
    {
        var entity = await _context.ConcreteRequests.FirstOrDefaultAsync(cr => cr.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"'{id}' ID'li beton talebi bulunamadı.");

        ValidateTransition(entity.Status, ConcreteRequestStatus.Approved);

        var finalAppointment =
            request.ApprovedAppointmentDateTime is { } appointment && appointment != default
                ? appointment
                : entity.RequestedDateTime;

        if (finalAppointment < DateTime.UtcNow)
            throw new ArgumentException(
                "Onaylanan randevu saati geçmişte olamaz. Lütfen geçerli bir saat girin veya talebi iptal edin.");

        entity.ApprovedAppointmentDateTime = finalAppointment;
        entity.Status = ConcreteRequestStatus.Approved;
        entity.ApprovedById = approvedByUserId;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Entry(entity).Property(e => e.RowVersion).OriginalValue = request.RowVersion;

        // Enqueue inside the same transaction so the approval and the outbox row land atomically.
        await _smsOutbox.EnqueueAsync(entity, SmsEventType.RequestApproved);

        await _context.SaveChangesAsync();

        var full = await LoadFullAsync(id);
        return ToDto(full!);
    }

    public async Task<ConcreteRequestDto> AssignVehicleAsync(Guid id, AssignVehicleRequest request, Guid assignedByUserId)
    {
        var entity = await _context.ConcreteRequests.FirstOrDefaultAsync(cr => cr.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"'{id}' ID'li beton talebi bulunamadı.");

        if (entity.Status != ConcreteRequestStatus.Approved)
            throw new ConflictException(
                $"Araç ataması yalnızca 'Onaylandı' durumundaki taleplerde yapılabilir. Mevcut durum: '{GetStatusLabel(entity.Status)}'.");

        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == request.VehicleId);
        if (vehicle is null)
            throw new KeyNotFoundException($"'{request.VehicleId}' ID'li araç bulunamadı.");

        if (vehicle.Status != VehicleStatus.Active)
            throw new ConflictException(
                $"'{vehicle.Plate}' plakalı araç aktif değil. Mevcut durum: '{GetVehicleStatusLabel(vehicle.Status)}'.");

        var alreadyAssigned = await _context.ConcreteRequestVehicles
            .AnyAsync(crv => crv.ConcreteRequestId == id && crv.VehicleId == request.VehicleId);
        if (alreadyAssigned)
            throw new ConflictException($"'{vehicle.Plate}' plakalı araç bu talebe zaten atanmış.");

        _context.ConcreteRequestVehicles.Add(new ConcreteRequestVehicle
        {
            Id = Guid.NewGuid(),
            ConcreteRequestId = entity.Id,
            VehicleId = vehicle.Id,
            CreatedAt = DateTime.UtcNow
        });

        entity.AssignedById = assignedByUserId;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Entry(entity).Property(e => e.RowVersion).OriginalValue = request.RowVersion;
        await _context.SaveChangesAsync();

        var full = await LoadFullAsync(id);
        return ToDto(full!);
    }

    public async Task RemoveVehicleAsync(Guid id, Guid vehicleId, uint rowVersion)
    {
        var entity = await _context.ConcreteRequests.FirstOrDefaultAsync(cr => cr.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"'{id}' ID'li beton talebi bulunamadı.");

        if (entity.Status != ConcreteRequestStatus.Approved)
            throw new ConflictException(
                $"Araç çıkarma yalnızca 'Onaylandı' durumundaki taleplerde yapılabilir. Mevcut durum: '{GetStatusLabel(entity.Status)}'.");

        var link = await _context.ConcreteRequestVehicles
            .FirstOrDefaultAsync(crv => crv.ConcreteRequestId == id && crv.VehicleId == vehicleId);
        if (link is null)
            throw new KeyNotFoundException($"'{vehicleId}' ID'li araç bu talebe atanmamış.");

        _context.ConcreteRequestVehicles.Remove(link);
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Entry(entity).Property(e => e.RowVersion).OriginalValue = rowVersion;
        await _context.SaveChangesAsync();
    }

    public async Task<ConcreteRequestDto> DeliverAsync(Guid id, DeliveryEntryRequest request, Guid deliveryRecordedByUserId)
    {
        if (request.DeliveredQuantity <= 0)
            throw new ArgumentException("Teslim edilen miktar sıfırdan büyük olmalıdır.");

        var entity = await _context.ConcreteRequests.FirstOrDefaultAsync(cr => cr.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"'{id}' ID'li beton talebi bulunamadı.");

        ValidateTransition(entity.Status, ConcreteRequestStatus.Delivered);

        entity.DeliveredQuantity = request.DeliveredQuantity;
        entity.Status = ConcreteRequestStatus.Delivered;
        entity.DeliveryRecordedById = deliveryRecordedByUserId;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Entry(entity).Property(e => e.RowVersion).OriginalValue = request.RowVersion;

        await _smsOutbox.EnqueueAsync(entity, SmsEventType.RequestDelivered);

        await _context.SaveChangesAsync();

        var full = await LoadFullAsync(id);
        return ToDto(full!);
    }

    public async Task<ConcreteRequestDto> CancelAsync(Guid id, CancelConcreteRequestRequest request, Guid cancelledByUserId)
    {
        var entity = await _context.ConcreteRequests.FirstOrDefaultAsync(cr => cr.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"'{id}' ID'li beton talebi bulunamadı.");

        ValidateTransition(entity.Status, ConcreteRequestStatus.Cancelled);

        entity.Status = ConcreteRequestStatus.Cancelled;
        entity.CancellationReason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
        entity.CancelledById = cancelledByUserId;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Entry(entity).Property(e => e.RowVersion).OriginalValue = request.RowVersion;
        await _context.SaveChangesAsync();

        var full = await LoadFullAsync(id);
        return ToDto(full!);
    }

    private static void ValidateTransition(ConcreteRequestStatus current, ConcreteRequestStatus target)
    {
        var allowed = (current, target) switch
        {
            (ConcreteRequestStatus.PendingApproval, ConcreteRequestStatus.Approved)  => true,
            (ConcreteRequestStatus.PendingApproval, ConcreteRequestStatus.Cancelled) => true,
            (ConcreteRequestStatus.Approved,        ConcreteRequestStatus.Delivered) => true,
            (ConcreteRequestStatus.Approved,        ConcreteRequestStatus.Cancelled) => true,
            _ => false
        };

        if (!allowed)
            throw new ConflictException(
                $"Talep '{GetStatusLabel(current)}' durumundan '{GetStatusLabel(target)}' durumuna geçirilemez.");
    }

    private async Task<ConcreteRequest?> LoadFullAsync(Guid id)
    {
        return await _context.ConcreteRequests
            .Include(cr => cr.Customer)
            .Include(cr => cr.Site)
            .Include(cr => cr.ConcreteRequestVehicles)
                .ThenInclude(crv => crv.Vehicle)
                    .ThenInclude(v => v.VehiclePersonnel)
                        .ThenInclude(vp => vp.Personnel)
            .Include(cr => cr.SmsLogs)
            .Include(cr => cr.CreatedBy)
            .Include(cr => cr.ApprovedBy)
            .Include(cr => cr.AssignedBy)
            .Include(cr => cr.DeliveryRecordedBy)
            .Include(cr => cr.CancelledBy)
            .FirstOrDefaultAsync(cr => cr.Id == id);
    }

    private static ConcreteRequestDto ToDto(ConcreteRequest cr)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var threshold = today.AddDays(MaintenanceAlertDays);

        var vehicles = cr.ConcreteRequestVehicles
            .Select(crv => ToAssignedVehicleDto(crv.Vehicle, today, threshold))
            .ToList();

        var smsLogs = cr.SmsLogs
            .OrderBy(sl => sl.CreatedAt)
            .Select(SmsLogMapper.ToDto)
            .ToList();

        return new ConcreteRequestDto(
            cr.Id,
            cr.Status,
            GetStatusLabel(cr.Status),
            cr.CustomerId,
            cr.Customer.CompanyName,
            cr.SiteId,
            cr.Site.Name,
            cr.Site.Address,
            cr.RequesterName,
            cr.CompanyContactPhone,
            cr.SiteContactPhone,
            cr.MaterialType,
            cr.RequestedQuantity,
            cr.UnitPrice,
            cr.TotalAmount,
            cr.WaybillType,
            cr.DeliveryMethod,
            cr.RequestedDateTime,
            cr.ApprovedAppointmentDateTime,
            cr.DeliveredQuantity,
            cr.Note,
            cr.CancellationReason,
            vehicles,
            smsLogs,
            ToUserDto(cr.CreatedBy),
            ToUserDto(cr.ApprovedBy),
            ToUserDto(cr.AssignedBy),
            ToUserDto(cr.DeliveryRecordedBy),
            ToUserDto(cr.CancelledBy),
            cr.CreatedAt,
            cr.RowVersion);
    }

    private static ConcreteRequestListDto ToListDto(ConcreteRequest cr)
    {
        var plates = cr.ConcreteRequestVehicles
            .Select(crv => crv.Vehicle.Plate)
            .OrderBy(p => p)
            .ToList();

        return new ConcreteRequestListDto(
            cr.Id,
            cr.Status,
            GetStatusLabel(cr.Status),
            cr.CustomerId,
            cr.Customer.CompanyName,
            cr.SiteId,
            cr.Site.Name,
            cr.MaterialType,
            cr.RequestedQuantity,
            cr.TotalAmount,
            cr.RequestedDateTime,
            cr.ApprovedAppointmentDateTime,
            plates,
            cr.CreatedAt,
            cr.RowVersion);
    }

    private static AssignedVehicleDto ToAssignedVehicleDto(Vehicle v, DateOnly today, DateOnly threshold)
    {
        var warning = v.Status == VehicleStatus.Active &&
                      v.NextMaintenanceDate.HasValue &&
                      v.NextMaintenanceDate.Value <= threshold;

        var personnel = v.VehiclePersonnel
            .Select(vp => new AssignedPersonnelDto(
                vp.Personnel.Id,
                vp.Personnel.FullName,
                vp.Personnel.Phone,
                vp.Personnel.Type,
                GetPersonnelTypeLabel(vp.Personnel.Type),
                vp.AssignmentType,
                GetAssignmentTypeLabel(vp.AssignmentType)))
            .ToList();

        return new AssignedVehicleDto(
            v.Id,
            v.Plate,
            v.Type,
            GetVehicleTypeLabel(v.Type),
            v.Status,
            GetVehicleStatusLabel(v.Status),
            warning,
            personnel);
    }

    private static ConcreteRequestUserDto? ToUserDto(User? user) =>
        user is null ? null : new ConcreteRequestUserDto(user.Id, user.FullName);

    private static string GetStatusLabel(ConcreteRequestStatus status) => status switch
    {
        ConcreteRequestStatus.PendingApproval => "Onay Bekleyen",
        ConcreteRequestStatus.Approved        => "Onaylandı",
        ConcreteRequestStatus.Delivered       => "Teslim Edildi",
        ConcreteRequestStatus.Cancelled       => "İptal Edildi",
        _                                     => status.ToString()
    };

    private static string GetVehicleTypeLabel(VehicleType type) => type switch
    {
        VehicleType.ConcreteMixer  => "Transmikser",
        VehicleType.Pump           => "Pompa",
        VehicleType.Excavator      => "Ekskavatör",
        VehicleType.SiteVehicle    => "Şantiye Aracı",
        VehicleType.ServiceVehicle => "Servis Aracı",
        _                          => type.ToString()
    };

    private static string GetVehicleStatusLabel(VehicleStatus status) => status switch
    {
        VehicleStatus.Active           => "Aktif",
        VehicleStatus.UnderMaintenance => "Bakımda",
        VehicleStatus.Passive          => "Pasif",
        VehicleStatus.OutOfSystem      => "Sistem Dışı",
        _                              => status.ToString()
    };

    private static string GetPersonnelTypeLabel(PersonnelType type) => type switch
    {
        PersonnelType.MixerDriver     => "Mikser Şoförü",
        PersonnelType.PumpOperator    => "Pompa Operatörü",
        PersonnelType.FieldPersonnel  => "Saha Personeli",
        PersonnelType.ServiceDriver   => "Servis Şoförü",
        _                             => type.ToString()
    };

    private static string GetAssignmentTypeLabel(PersonnelAssignmentType type) => type switch
    {
        PersonnelAssignmentType.Primary => "Asıl",
        PersonnelAssignmentType.Backup  => "Yedek",
        _                                => type.ToString()
    };
}
