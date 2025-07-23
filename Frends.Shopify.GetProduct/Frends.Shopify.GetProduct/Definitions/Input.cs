using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetProduct.Definitions;

/// <summary>
/// Input parameters for Shopify GetProduct task.
/// </summary>
public class Input
{
    /// <summary>
    /// The ID of the product to retrieve.
    /// </summary>
    /// <example>1234567890123</example>
    [Display(Name = "Product Id")]
    public string ProductId { get; set; }
}
