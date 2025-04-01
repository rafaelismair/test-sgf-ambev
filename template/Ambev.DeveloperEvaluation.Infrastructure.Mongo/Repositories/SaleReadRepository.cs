using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ReadModels;
using Ambev.DeveloperEvaluation.Infrastructure.Mongo.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Infrastructure.Mongo.Repositories;
    public interface ISaleReadRepository
    {
        Task AddAsync(SaleReadModel sale, CancellationToken cancellationToken);
    }

public class SaleReadRepository : ISaleReadRepository
{
    private readonly MongoDB.Driver.IMongoCollection<SaleReadModel> _collection;

    public SaleReadRepository(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<SaleReadModel>("sales");
    }

    public async Task AddAsync(SaleReadModel sale, CancellationToken cancellationToken)
        => await _collection.InsertOneAsync(sale, null, cancellationToken);
}




