using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.CreateProduct.Definitions;

/// <summary>
/// Input parameters for Shopify CreateProduct task.
/// </summary>
public class Input
{
    /// <summary>
    /// A JSON object containing the product details to create.
    /// </summary>
    /// <example>
    /// <code>
    /// Example test product with variants:
    /// new JObject
    /// {
    ///     ["title"] = "Test Product",
    ///     ["body_html"] = "<p>Test description</p>",
    ///     ["vendor"] = "Test Vendor",
    ///     ["product_type"] = "Test Type",
    ///     ["variants"] = new JArray
    ///     {
    ///         new JObject
    ///         {
    ///             ["option1"] = "Size",
    ///             ["price"] = "10.99",
    ///             ["sku"] = "TEST-SIZE"
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    [Display(Name = "Product Data")]
    public JObject ProductData { get; set; }
}
