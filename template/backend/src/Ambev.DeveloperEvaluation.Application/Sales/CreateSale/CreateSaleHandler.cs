using Ambev.DeveloperEvaluation.Application.Eventing;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Infrastructure.Mongo.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly IEventPublisher _eventPublisher;
    private readonly ISaleReadRepository _saleReadRepository;


    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The ILogger instance</param>
    /// <param name="eventPublisher">The EventPublisher instance to publish events for sale</param>
    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger,
        IEventPublisher eventPublisher,
        ISaleReadRepository saleReadRepository)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _eventPublisher = eventPublisher;
        _saleReadRepository = saleReadRepository;

    }


    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new sale for customer {Customer}", command.Customer);

        var sale = new SaleEntity
        {
            Customer = command.Customer,
            Branch = command.Branch,
            Products = command.Products.Select(p => {
                ValidateProductQuantity(command.Customer, p.Quantity);

                var productSale = new ProductSale
                {
                    Name = p.Name,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice
                };

                ApplyDiscount(productSale);
                return productSale;
            }).ToList()
        };

        sale.CalculateTotalSaleAmount();

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        var readModel = new Domain.ReadModels.SaleReadModel
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.Id.ToString(), // ou outro número real
            Date = DateTime.UtcNow,
            Customer = createdSale.Customer,
            Branch = createdSale.Branch,
            TotalAmount = createdSale.TotalSaleAmount,
            IsCancelled = false,
            Items = createdSale.Products.Select(p => new Domain.ReadModels.SaleItemReadModel
            {
                Product = p.Name,
                Quantity = p.Quantity,
                UnitPrice = p.UnitPrice,
                Total = p.Quantity * p.UnitPrice
            }).ToList()
        };

        await _saleReadRepository.AddAsync(readModel, cancellationToken);


        return new CreateSaleResult { Id = createdSale.Id };
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

    private async Task PublishSaleCreatedEventAsync(SaleEntity createdSale, CancellationToken cancellationToken)
    {
        var saleCreatedEvent = new SaleCreatedEvent(createdSale.Id, createdSale.Customer, createdSale.TotalSaleAmount);
        await _eventPublisher.PublishAsync(saleCreatedEvent, cancellationToken);
    }

  
}