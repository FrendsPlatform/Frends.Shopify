namespace Frends.Shopify.DeleteProduct.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the product was deleted successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// Result of deleting a Shopify product.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="error">Error details if the operation failed.</param>
    internal Result(bool success, Error error = null)
    {
        Success = success;
        Error = error;
    }
}
