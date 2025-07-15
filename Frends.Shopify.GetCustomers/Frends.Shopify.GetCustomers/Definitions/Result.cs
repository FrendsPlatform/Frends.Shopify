using System.Collections.Generic;

namespace Frends.Shopify.GetCustomers.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// Result of retrieving Shopify customers.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="customers">The retrieved customers data.</param>
    /// <param name="pageInfo">Pagination information.</param>
    /// <param name="error">Error details if the operation failed.</param>
    internal Result(bool success, List<object> customers, PageInfo pageInfo = null, Error error = null)
    {
        Success = success;
        Customers = customers;
        PageInfo = pageInfo;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the retrieval was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// List of customer objects returned from Shopify.
    /// </summary>
    public List<object> Customers { get; set; }

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