namespace Frends.Shopify.GetProduct.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
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

    internal Result(bool success, object product, Error error = null)
    {
        Success = success;
        Product = product;
        Error = error;
    }
}