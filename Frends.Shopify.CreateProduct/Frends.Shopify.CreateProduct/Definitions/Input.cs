using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.CreateProduct.Definitions;

/// <summary>
/// Input parameters for Shopify CreateProduct task.
/// </summary>
public class Input
{
    /// <summary>
    /// A JSON object containing the product details to create.
    /// </summary>
    [Display(Name = "Product Data")]
    [DisplayFormat(DataFormatString = "Json")]
    public object ProductData { get; set; }
}
