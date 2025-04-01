using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.Domain.ReadModels;

public class SaleReadModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = default!;
    public DateTime Date { get; set; }
    public string Customer { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public List<SaleItemReadModel> Items { get; set; } = new();
}

public class SaleItemReadModel
{
    public string Product { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}
