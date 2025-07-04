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
internal class RealShopifyApiClient : IShopifyApiClient, IDisposable
{
    private readonly Connection connection;
    private readonly HttpClient httpClient;
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="RealShopifyApiClient"/> class.
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    public RealShopifyApiClient(Connection connection)
    {
        this.connection = connection;
        httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://{this.connection.ShopName}.myshopify.com/admin/api/{this.connection.ApiVersion}/"),
        };
        httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", this.connection.AccessToken);
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
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the HttpClient resource.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            httpClient?.Dispose();
        }

        disposed = true;
    }
}