using Ambev.DeveloperEvaluation.Application.Eventing;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ReadModels;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Dto;
using Ambev.DeveloperEvaluation.Infrastructure.Mongo.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sale;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleReadRepository _saleReadRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly IEventPublisher _eventPublisher;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _saleReadRepository = Substitute.For<ISaleReadRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _eventPublisher = Substitute.For<IEventPublisher>();

        _handler = new CreateSaleHandler(
            _saleRepository,
            _mapper,
            _logger,
            _eventPublisher,
            _saleReadRepository
        );
    }

    [Fact(DisplayName = "Handle should create sale and return successfully with ID")]
    public async Task Handle_Should_Create_Sale_And_Return_Result_Success_With_Data()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            Customer = "Customer1",
            Branch = "Branch1",
            Products = new List<CreateProductSaleDto>
            {
                new CreateProductSaleDto { Name = "Product1", Quantity = 5, UnitPrice = 10.0M }
            }
        };

        var fakeSale = new SaleEntity
        {
            Id = Guid.NewGuid(),
            Customer = command.Customer,
            Branch = command.Branch,
            Products = new List<ProductSale>
            {
                new ProductSale { Name = "Product1", Quantity = 5, UnitPrice = 9.0M } // com desconto aplicado
            }
        };

        _saleRepository.CreateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>())
            .Returns(fakeSale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fakeSale.Id, result.Id);

        // Verify interactions
        await _saleRepository.Received(1).CreateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>());
        await _saleReadRepository.Received(1).AddAsync(Arg.Any<SaleReadModel>(), Arg.Any<CancellationToken>());
    }
}
