using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleHandler requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The update sale details</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (sale == null)
        {
            throw new KeyNotFoundException("Sale not found");
        }

        sale.Customer = command.Customer;
        sale.Branch = command.Branch;
        sale.Products = command.Products.Select(p => new ProductSale
        {
            Name = p.Name,
            Quantity = p.Quantity,
            UnitPrice = p.UnitPrice
        }).ToList();
        sale.IsCanceled = command.IsCanceled;

        sale.CalculateTotalSaleAmount();

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        return new UpdateSaleResult { Id = updatedSale.Id };
    }
}