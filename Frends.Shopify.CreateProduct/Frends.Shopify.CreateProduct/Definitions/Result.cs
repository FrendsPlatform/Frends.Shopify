using Newtonsoft.Json.Linq;

namespace Frends.Shopify.CreateProduct.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// Result of creating a Shopify product.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="createdProduct">The created product data if successful.</param>
    /// <param name="error">Error details if the operation failed.</param>
    internal Result(bool success, JObject createdProduct, Error error = null)
    {
        Success = success;
        CreatedProduct = createdProduct;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the retrieval was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The created product object returned by Shopify.
    /// </summary>
    /// <example>
    /// {
    ///   "id": 1234567890123,
    ///   "title": "Example Product",
    ///   "body_html": "Example Description",
    ///   "vendor": "Example Vendor",
    ///   "product_type": "Example Type",
    ///   "status": "active",
    ///   "variants": [
    ///     {
    ///       "id": 987654321,
    ///       "title": "Default Title",
    ///       "price": "19.99",
    ///       "sku": "EX-001"
    ///     }
    ///   ],
    ///   "options": [
    ///     {
    ///       "name": "Title",
    ///       "values": ["Default Title"]
    ///     }
    ///   ]
    /// }
    /// </example>
    public JObject CreatedProduct { get; private set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}