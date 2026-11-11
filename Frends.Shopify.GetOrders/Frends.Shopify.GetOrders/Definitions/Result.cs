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
    /// Indicates whether the retrieval was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// List of order objects returned from Shopify.
    /// </summary>
    /// <example>
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
    /// </example>
    public List<object> Orders { get; set; }

    /// <summary>
    /// Pagination cursor.
    /// </summary>
    /// <example>object { string NextPage, string PreviousPage }</example>
    public PageInfo PageInfo { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}

/// <summary>
/// Pagination cursor.
/// </summary>
public class PageInfo
{
    /// <summary>
    /// Pagination cursor for nextPage.
    /// </summary>
    public string NextPage { get; set; }

    /// <summary>
    /// Pagination cursor for previousPage.
    /// </summary>
    public string PreviousPage { get; set; }
}