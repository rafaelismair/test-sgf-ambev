using Ambev.DeveloperEvaluation.Dto;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents a request to create a new sale in the system.
/// </summary>
public class CreateSaleRequest
{
    /// <summary>
    /// The customer who purchased
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// Where the sale was made
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// The products of sale
    /// </summary>
    public List<CreateProductSaleDto> Products { get; set; } = new List<CreateProductSaleDto>();
}