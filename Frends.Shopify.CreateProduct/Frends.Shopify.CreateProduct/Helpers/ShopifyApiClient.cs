using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.CreateProduct.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.CreateProduct.Helpers;

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
        if (string.IsNullOrWhiteSpace(connection.ShopName)) throw new ArgumentException("ShopName cannot be null or empty.", nameof(connection));
        if (string.IsNullOrWhiteSpace(connection.ApiVersion)) throw new ArgumentException("ApiVersion cannot be null or empty.", nameof(connection));
        if (string.IsNullOrWhiteSpace(connection.AccessToken)) throw new ArgumentException("AccessToken cannot be null or empty.", nameof(connection));

        httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/"),
        };
        httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);
    }

    /// <summary>
    /// Creates a new product in Shopify.
    /// </summary>
    /// <param name="productData">Product data as JObject.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>JObject containing the Shopify API response with created product details.</returns>
    public async Task<JObject> CreateProductAsync(JObject productData, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("products.json", new { product = productData }, cancellationToken);
        response.EnsureSuccessStatusCode();
        return JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
    }

    /// <summary>
    /// Disposes the HttpClient resource.
    /// </summary>
    public void Dispose()
    {
        httpClient?.Dispose();
    }
}