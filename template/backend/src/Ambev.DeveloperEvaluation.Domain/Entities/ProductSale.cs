namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product of sale in the system.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class ProductSale
{
    /// <summary>
    /// The unique identifier of the product sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The id of sale
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// The id of product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of product
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of product
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// The total amount of the products being the quantity * the unit price
    /// </summary>
    public decimal TotalAmount { get; set; }
}