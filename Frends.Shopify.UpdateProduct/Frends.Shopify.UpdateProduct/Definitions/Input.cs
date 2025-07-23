using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.UpdateProduct.Definitions;

/// <summary>
///  Input parameters for Shopify UpdateProduct task.
/// </summary>
public class Input
{
    /// <summary>
    /// The unique identifier of the product to be updated.
    /// </summary>
    [Display(Name = "Product Id")]
    public string ProductId { get; set; }

    /// <summary>
    /// A JSON object representing the fields to update (e.g., title, body_html, variants, images, etc.).
    /// </summary>
    [Display(Name = "Product Data")]
    public JObject ProductData { get; set; }
}
