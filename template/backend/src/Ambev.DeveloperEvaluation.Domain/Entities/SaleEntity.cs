namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale in the system.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class SaleEntity
{
    /// <summary>
    /// The unique identifier of the sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The date of the sale
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The customer who purchased
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// The total purchase price
    /// </summary>
    public decimal TotalSaleAmount { get; set; }

    /// <summary>
    /// Where the sale was made
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// The products of sale
    /// </summary>
    public List<ProductSale> Products { get; set; } = new List<ProductSale>();

    /// <summary>
    /// Sale is canceled true or false
    /// </summary>
    public bool IsCanceled { get; set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public SaleEntity()
    {
        Date = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the total sale amount based on products
    /// </summary>
    public void CalculateTotalSaleAmount()
    {
        TotalSaleAmount = Products.Sum(p => p.Quantity * p.UnitPrice);
    }
}