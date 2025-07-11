using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetOrders.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetOrders.Helpers;

/// <summary>
/// Implementation of IShopifyApiClient that communicates with actual Shopify API.
/// </summary>
internal class ShopifyApiClient : IShopifyApiClient, IDisposable
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopifyApiClient"/> class.
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    public ShopifyApiClient(Connection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (string.IsNullOrWhiteSpace(connection.ShopDomain)) throw new ArgumentException("ShopDomain is required");
        if (string.IsNullOrWhiteSpace(connection.AccessToken)) throw new ArgumentException("AccessToken is required");

        httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://{connection.ShopDomain}/admin/api/2023-10/"),
        };
        httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);
    }

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
    public async Task<OrdersResponse> GetOrdersAsync(string createdAtMin, string createdAtMax, string status, string fulfillmentStatus, string fields, int limit, string pageInfo, CancellationToken cancellationToken)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(createdAtMin))
            queryParams.Add($"created_at_min={Uri.EscapeDataString(createdAtMin)}");

        if (!string.IsNullOrEmpty(createdAtMax))
            queryParams.Add($"created_at_max={Uri.EscapeDataString(createdAtMax)}");

        if (!string.IsNullOrEmpty(status) && status != "any")
            queryParams.Add($"status={status}");

        if (!string.IsNullOrEmpty(fulfillmentStatus))
            queryParams.Add($"fulfillment_status={fulfillmentStatus}");

        if (!string.IsNullOrEmpty(fields))
            queryParams.Add($"fields={Uri.EscapeDataString(fields)}");

        if (limit > 0)
            queryParams.Add($"limit={limit}");

        if (!string.IsNullOrEmpty(pageInfo))
            queryParams.Add($"page_info={Uri.EscapeDataString(pageInfo)}");

        var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
        var url = $"orders.json{queryString}";

        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var json = JObject.Parse(responseContent);

        var pageInfoHeader = response.Headers.TryGetValues("Link", out var linkHeader)
            ? ParseLinkHeader(linkHeader)
            : null;

        return new OrdersResponse
        {
            Orders = (JArray)json["orders"],
            PageInfo = pageInfoHeader,
        };
    }

    /// <summary>
    /// Disposes the HttpClient resource.
    /// </summary>
    public void Dispose()
    {
        httpClient?.Dispose();
    }

    /// <summary>
    /// Parses Shopify's Link header to extract pagination cursors.
    /// </summary>
    /// <param name="linkHeader">The Link header value(s) from the HTTP response</param>
    /// <returns>PageInfo pageInfo { string NextPage, string PreviousPage }</returns>
    private static PageInfo ParseLinkHeader(IEnumerable<string> linkHeader)
    {
        var pageInfo = new PageInfo();

        foreach (var link in linkHeader)
        {
            var links = link.Split(',');
            foreach (var item in links)
            {
                if (item.Contains("rel=\"next\""))
                {
                    var start = item.IndexOf("page_info=", StringComparison.Ordinal) + "page_info=".Length;
                    var end = item.IndexOf('>', start);
                    if (start > 0 && end > start)
                        pageInfo.NextPage = item[start..end];
                }
                else if (item.Contains("rel=\"previous\""))
                {
                    var start = item.IndexOf("page_info=", StringComparison.Ordinal) + "page_info=".Length;
                    var end = item.IndexOf('>', start);
                    if (start > 0 && end > start)
                        pageInfo.PreviousPage = item[start..end];
                }
            }
        }

        return pageInfo;
    }
}