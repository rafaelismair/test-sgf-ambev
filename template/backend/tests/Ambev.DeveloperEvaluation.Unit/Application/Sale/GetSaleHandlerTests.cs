using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sale;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates real mapper configuration.
    /// </summary>
    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();

        // Configuração real do AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SaleEntity, GetSaleResult>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests for return all sales.
    /// </summary>
    [Fact(DisplayName = "Handle should return all sales")]
    public async Task Handle_Should_Return_AllSales()
    {
        // Arrange
        var sales = new List<SaleEntity>
         {
             new SaleEntity { Id = Guid.NewGuid(), Customer = "Customer1", TotalSaleAmount = 100 },
             new SaleEntity { Id = Guid.NewGuid(), Customer = "Customer2", TotalSaleAmount = 200 }
         };

        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(sales);

        // Act
        var result = await _handler.Handle(new GetSaleCommand(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(sales[0].Id, result[0].Id);
        Assert.Equal(sales[0].Customer, result[0].Customer);
        Assert.Equal(sales[1].Id, result[1].Id);
        Assert.Equal(sales[1].Customer, result[1].Customer);
    }
}