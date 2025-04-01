using Ambev.DeveloperEvaluation.Application.Eventing;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleHandler requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository"></param>
    /// <param name="mapper"></param>
    /// <param name="logger"></param>
    /// <param name="eventPublisher"></param>
    public UpdateSaleHandler(
         ISaleRepository saleRepository, 
         IMapper mapper, 
         ILogger<UpdateSaleHandler> logger, 
         IEventPublisher eventPublisher)
     {
         _saleRepository = saleRepository;
         _mapper = mapper;
         _logger = logger;
         _eventPublisher = eventPublisher;
     }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The update sale details</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating sale with ID {SaleId}", command.Id);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (sale == null)
        {
            _logger.LogError("Sale with ID {SaleId} not found to update", command.Id);
            throw new KeyNotFoundException("Sale not found");
        }

        sale.Customer = command.Customer;
        sale.Branch = command.Branch;
        sale.Products = command.Products.Select(p => {
            ValidateProductQuantity(command.Customer, p.Quantity);

            var productSale = new ProductSale
            {
                Name = p.Name,
                Quantity = p.Quantity,
                UnitPrice = p.UnitPrice
            };

            ApplyDiscount(productSale);
            return productSale;
        }).ToList();

        sale.IsCanceled = command.IsCanceled;

        sale.CalculateTotalSaleAmount();

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        _logger.LogInformation("Sale with ID {SaleId} updated successfully", updatedSale.Id);

        await PublishSaleUpdatedEventAsync(updatedSale, cancellationToken);

        return new UpdateSaleResult { Id = updatedSale.Id };
    }

    private void ValidateProductQuantity(string customer, int quantity)
    {
        if (quantity > 20)
        {
            _logger.LogInformation("Customer {Customer} is attempting to purchase more than 20 identical items.", customer);

            throw new InvalidOperationException("Is not possible to sell more than 20 identical items.");
        }
    }

    private void ApplyDiscount(ProductSale productSale)
    {
        if (productSale.Quantity >= 4 && productSale.Quantity < 10)
        {
            productSale.UnitPrice *= 0.9m; // 10% discount
        }
        else if (productSale.Quantity >= 10 && productSale.Quantity <= 20)
        {
            productSale.UnitPrice *= 0.8m; // 20% discount
        }
    }

    private async Task PublishSaleUpdatedEventAsync(SaleEntity updatedSale, CancellationToken cancellationToken)
    {
        var saleUpdatedEvent = new SaleUpdatedEvent(updatedSale.Id, updatedSale.Customer, updatedSale.TotalSaleAmount, updatedSale.IsCanceled);
        await _eventPublisher.PublishAsync(saleUpdatedEvent, cancellationToken);
    }
}