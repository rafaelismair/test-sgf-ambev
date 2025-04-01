using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.ReadModels;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Infrastructure.Mongo.Repositories;
using MongoDB.Driver;
using Xunit;
using Microsoft.Extensions.Logging;
using AutoMapper;
using NSubstitute;
using Ambev.DeveloperEvaluation.Application.Eventing;
using Ambev.DeveloperEvaluation.Dto;
using FluentAssertions;

namespace Ambev.DeveloperEvaluation.Integration;

public class CreateSaleIntegrationTests
{
    private readonly IMongoCollection<SaleReadModel> _collection;
    private readonly CreateSaleHandler _handler;

    public CreateSaleIntegrationTests()
    {

        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("SalesDb");
        _collection = database.GetCollection<SaleReadModel>("sales");


        _collection.DeleteMany(_ => true);


        var saleRepository = Substitute.For<ISaleRepository>();
        var eventPublisher = Substitute.For<IEventPublisher>();
        var mapper = Substitute.For<IMapper>();
        var logger = Substitute.For<ILogger<CreateSaleHandler>>();
        var saleReadRepository = new SaleReadRepository(
            Microsoft.Extensions.Options.Options.Create(new Infrastructure.Mongo.Settings.MongoSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "SalesDb"
            })
        );

        // Setup do handler real
        _handler = new CreateSaleHandler(
            saleRepository,
            mapper,
            logger,
            eventPublisher,
            saleReadRepository
        );

        // Setup simples para simular salvamento (ajuste conforme seu domínio)
        saleRepository.CreateAsync(Arg.Any<Domain.Entities.SaleEntity>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Domain.Entities.SaleEntity>());
    }

    [Fact]
    public async Task CreateSale_ShouldSaveReadModelInMongo()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            Customer = "Customer",
            Branch = "Branch",
            Products = new List<CreateProductSaleDto>
            {
                new() { Name = "Product A", Quantity = 5, UnitPrice = 10.0m }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var saved = await _collection.Find(x => x.Customer == "Customer").FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Items.Should().HaveCount(1);
    }
}
