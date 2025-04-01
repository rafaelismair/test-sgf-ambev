namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCreatedEvent(Guid SaleId, string Customer, decimal TotalAmount);
public record SaleUpdatedEvent(Guid SaleId, string Customer, decimal TotalAmount, bool IsCanceled);
public record SaleDeletedEvent(Guid SaleId);

