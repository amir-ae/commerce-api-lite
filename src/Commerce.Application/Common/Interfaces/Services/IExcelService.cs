using Commerce.Domain.Common.ValueObjects;
using ErrorOr;

namespace Commerce.Application.Common.Interfaces.Services;

public interface IExcelService
{
     Task<ErrorOr<MemoryStream>> InvoiceById(OrderId orderId, CentreId centreId, CancellationToken ct = default);
     Task<ErrorOr<MemoryStream>> UnrepairableById(OrderId orderId, CentreId centreId, CancellationToken ct = default);
     Task<ErrorOr<MemoryStream>> Report(CentreId? centreId, DateTimeOffset? startDate, DateTimeOffset? endDate, CancellationToken ct = default);
}