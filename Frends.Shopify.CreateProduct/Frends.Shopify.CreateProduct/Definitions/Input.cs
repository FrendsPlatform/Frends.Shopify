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
    /// <example>
    /// <code>
    /// new JObject
    /// {
    ///     ["product"] = new JObject
    ///     {
    ///         ["title"] = "Test Product",
    ///         ["body_html"] = "Product Description",
    ///         ["vendor"] = "Test Vendor",
    ///         ["product_type"] = "Test Type",
    ///         ["variants"] = new JArray
    ///         {
    ///             new JObject
    ///             {
    ///                 ["option1"] = "Default",
    ///                 ["price"] = "9.99",
    ///                 ["sku"] = "PROD-001"
    ///             }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    [Display(Name = "Product Data")]
    public object ProductData { get; set; }
}
