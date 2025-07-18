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
    /// <example>1234567890123</example>
    [Display(Name = "Product Id")]
    public string ProductId { get; set; }

    /// <summary>
    /// A JSON object representing the fields to update (e.g., title, body_html, variants, images, etc.).
    /// </summary>
    /// <example>
    /// <code>
    /// new JObject
    /// {
    ///     ["title"] = "Test Product",
    ///     ["body_html"] = "Product Description",
    ///     ["vendor"] = "Test Vendor",
    ///     ["product_type"] = "Test Type",
    ///     ["variants"] = new JArray
    ///     {
    ///         new JObject
    ///         {
    ///             ["option1"] = "Default",
    ///             ["price"] = "9.99",
    ///             ["sku"] = "PROD-001"
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    [Display(Name = "Product Data")]
    public JObject ProductData { get; set; }
}
