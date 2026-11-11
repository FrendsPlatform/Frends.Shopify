using System.Collections.Generic;

namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// Result of retrieving Shopify orders.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="orders">The retrieved orders data.</param>
    /// <param name="pageInfo">Pagination information.</param>
    /// <param name="error">Error details if the operation failed.</param>
    internal Result(bool success, List<object> orders, PageInfo pageInfo = null, Error error = null)
    {
        Success = success;
        Orders = orders;
        PageInfo = pageInfo;
        Error = error;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the retrieval was successful.
    /// </summary>
    /// <example><code>true</code></example>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the list of order objects returned from Shopify.
    /// </summary>
    /// <example>
    /// <code>
    /// [
    ///   {
    ///     "id": 1234567890123,
    ///     "email": "customer@example.com",
    ///     "created_at": "2025-01-01T12:00:00Z",
    ///     "updated_at": "2025-01-01T12:00:00Z",
    ///     "number": 1001,
    ///     "status": "open",
    ///     "total_price": "10.00",
    ///     "currency": "EUR",
    ///     "customer": {
    ///       "id": 1234567890123,
    ///       "first_name": "Example",
    ///       "last_name": "Name"
    ///     },
    ///     "line_items": [
    ///       {
    ///         "id": 1234567890123,
    ///         "product_id": 1234567890123,
    ///         "name": "Example Product",
    ///         "price": "10.00",
    ///         "quantity": 1
    ///       }
    ///     ]
    ///   }
    /// ]
    /// </code>
    /// </example>
    public List<object> Orders { get; set; }

    /// <summary>
    /// Gets or sets the pagination cursors.
    /// </summary>
    /// <example><code>PageInfo { NextPage = "next-page-cursor", PreviousPage = "previous-page-cursor" }</code></example>
    public PageInfo PageInfo { get; set; }

    /// <summary>
    /// Gets or sets the error that occurred during task execution.
    /// </summary>
    /// <example>
    /// <code>
    /// Error { Message = "Shopify API request failed", AdditionalInfo = new Exception("Invalid access token") }
    /// </code>
    /// </example>
    public Error Error { get; set; }
}
