using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetProduct.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetProduct.Helpers;

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
        if (string.IsNullOrWhiteSpace(connection.ShopName)) throw new ArgumentException("ShopName is required");
        if (string.IsNullOrWhiteSpace(connection.AccessToken)) throw new ArgumentException("AccessToken is required");
        if (string.IsNullOrWhiteSpace(connection.ApiVersion)) throw new ArgumentException("ApiVersion is required");

        httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/"),
        };
        httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);
    }

    /// <summary>
    /// Retrieves a product from the Shopify store.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve. Must be a valid Shopify product ID (numeric string).</param>
    /// <param name="fields">Optional array of fields included in the response.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>A Task that returns JToken containing the product data.</returns>
    public async Task<JToken> GetProductAsync(string productId, string[] fields, CancellationToken cancellationToken)
    {
        var url = $"products/{productId}.json";

        if (fields != null && fields.Length > 0)
        {
            url += $"?fields={string.Join(",", fields)}";
        }

        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JObject.Parse(responseContent)["product"];
    }

    /// <summary>
    /// Disposes the HttpClient resource.
    /// </summary>
    public void Dispose()
    {
        httpClient?.Dispose();
    }
}