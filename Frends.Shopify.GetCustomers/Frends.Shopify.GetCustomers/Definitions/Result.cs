using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetCustomers.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// Result of retrieving a Shopify customer.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="customer">The retrieved customer data.</param>
    /// <param name="error">Error details if the operation failed.</param>
    internal Result(bool success, JObject customer, Error error = null)
    {
        Success = success;
        Customer = customer;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the retrieval was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The customer object returned by Shopify.
    /// </summary>
    public JObject Customer { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}