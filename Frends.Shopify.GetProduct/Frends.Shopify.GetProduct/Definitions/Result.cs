namespace Frends.Shopify.GetProduct.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// Result of retrieving a Shopify product.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="product">The retrieved product data.</param>
    /// <param name="error">Error details if the operation failed.</param>
    internal Result(bool success, object product, Error error = null)
    {
        Success = success;
        Product = product;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the retrieval was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The product object returned by Shopify.
    /// </summary>
    /// <example>foobar,foobar</example>
    public object Product { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}