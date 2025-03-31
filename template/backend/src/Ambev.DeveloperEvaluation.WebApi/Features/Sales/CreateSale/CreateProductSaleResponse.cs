namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// API response model for CreateProductSaleResponse operation
/// </summary>
public class CreateProductSaleResponse
{
    /// <summary>
    /// The name of product
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