using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Dto;
using AutoMapper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sale;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests for update sale and return successfuly with id.
    /// </summary>
    [Fact(DisplayName = "Handle should update sale and return successfuly with id")]
    public async Task Handle_Should_Update_Sale_And_Return_Result_Success_With_Data()
    {
        // Arrange
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            Customer = "UpdatedCustomer",
            Branch = "UpdatedBranch",
            Products = new List<UpdateProductSaleDto>
             {
                 new UpdateProductSaleDto { Name = "UpdatedProduct", Quantity = 5, UnitPrice = 15.0M }
             },
            IsCanceled = true
        };

        var saleEntity = new SaleEntity
        {
            Id = command.Id,
            Customer = "OriginalCustomer",
            Branch = "OriginalBranch"
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
          .Returns(saleEntity);

        _saleRepository.UpdateAsync(Arg.Any<SaleEntity>(), Arg.Any<CancellationToken>())
         .Returns(saleEntity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.Id, result.Id);
    }
}