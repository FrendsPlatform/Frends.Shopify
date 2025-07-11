using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetOrders.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetOrders.Helpers;

/// <summary>
/// Interface for Shopify API implementations.
/// </summary>
public interface IShopifyApiClient
{
    /// <summary>
    /// Retrieves orders from Shopify.
    /// </summary>
    /// <param name="createdAtMin">Optional start date/time in ISO 8601 format.</param>
    /// <param name="createdAtMax">Optional end date/time in ISO 8601 format.</param>
    /// <param name="status">Optional filter for orders by status: open, closed, cancelled, or any. Default is any.</param>
    /// <param name="fulfillmentStatus">Optional filter by fulfillment status, e.g., shipped, partial, unshipped.</param>
    /// <param name="fields">Optional comma-separated list of fields to return.</param>
    /// <param name="limit">Optional max number of results per page (1–250). Default is 50.</param>
    /// <param name="pageInfo">Optional pagination cursor (nextPage or previousPage).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task that resolves to an OrdersResponse containing orders and page info.</returns>
    Task<OrdersResponse> GetOrdersAsync(string createdAtMin, string createdAtMax, string status, string fulfillmentStatus, string fields, int limit, string pageInfo, CancellationToken cancellationToken);
}

/// <summary>
/// Response container for orders and pagination info
/// </summary>
public class OrdersResponse
{
    /// <summary>
    /// List of order objects returned from Shopify.
    /// </summary>
    public JArray Orders { get; set; }

    /// <summary>
    /// Pagination cursor (nextPage or previousPage).
    /// </summary>
    public PageInfo PageInfo { get; set; }
}