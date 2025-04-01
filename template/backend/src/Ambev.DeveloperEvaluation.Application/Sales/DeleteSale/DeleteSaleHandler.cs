using Ambev.DeveloperEvaluation.Application.Eventing;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleHandler requests
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<DeleteSaleHandler> _logger;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of DeleteUserHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>s
    public DeleteSaleHandler(
         ISaleRepository saleRepository,
         ILogger<DeleteSaleHandler> logger,
         IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the DeleteSaleCommand request
    /// </summary>
    /// <param name="request">The DeleteSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var success = await _saleRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
        {
            _logger.LogError("Sale with ID {SaleId} not found to delete", request.Id);

            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");
        }
        _logger.LogInformation("Sale with ID {SaleId} deleted successfully", request.Id);

        await PublishSaleDeletedEventAsync(request.Id, cancellationToken);

        return new DeleteSaleResponse { Success = true };
    }

    private async Task PublishSaleDeletedEventAsync(Guid SaleId, CancellationToken cancellationToken)
    {
        var saleDeletedEvent = new SaleDeletedEvent(SaleId);
        await _eventPublisher.PublishAsync(saleDeletedEvent, cancellationToken);
    }
}