using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.DeleteProduct.Definitions;

/// <summary>
/// Input parameters for Shopify DeleteProduct task.
/// </summary>
public class Input
{
    /// <summary>
    /// The ID of the product to delete.
    /// </summary>
    [Display(Name = "Product Id")]
    public string ProductId { get; set; }
}
