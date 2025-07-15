using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetCustomers.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetCustomers.Helpers;

/// <summary>
/// Interface for Shopify API implementations.
/// </summary>
public interface IShopifyApiClient
{
    /// <summary>
    /// Retrieves customers from Shopify.
    /// </summary>
    /// <param name="createdAtMin">Optional start date/time in ISO 8601 format.</param>
    /// <param name="createdAtMax">Optional end date/time in ISO 8601 format.</param>
    /// <param name="state">Optional filter by customer state: ENABLED, DISABLED, INVITED, DECLINED.</param>
    /// <param name="fields">Optional comma-separated list of fields to return.</param>
    /// <param name="limit">Optional max number of results per page (1–250). Default is 50.</param>
    /// <param name="pageInfo">Optional pagination cursor (nextPage or previousPage).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task that resolves to a CustomersResponse containing customers and page info.</returns>
    Task<CustomersResponse> GetCustomersAsync(string createdAtMin, string createdAtMax, string state, string fields, int limit, string pageInfo, CancellationToken cancellationToken);
}

/// <summary>
/// Response container for customers and pagination info
/// </summary>
public class CustomersResponse
{
    /// <summary>
    /// List of customer objects returned from Shopify.
    /// </summary>
    public JArray Customers { get; set; }

    /// <summary>
    /// Pagination cursor (nextPage or previousPage).
    /// </summary>
    public PageInfo PageInfo { get; set; }
}