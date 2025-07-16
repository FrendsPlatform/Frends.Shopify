using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.UpdateProduct.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.UpdateProduct.Helpers;

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
        httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/"),
        };
        httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);
    }

    /// <summary>
    /// Updates an existing product in Shopify.
    /// </summary>
    /// <param name="productId">The ID of the product to update. Must be a valid Shopify product ID.</param>
    /// <param name="productData">Product data as JObject containing the fields to update.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>A Task that represents the asynchronous update operation.</returns>
    public async Task UpdateProductAsync(string productId, JObject productData, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"products/{productId}.json", new { product = productData }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Disposes the HttpClient resource.
    /// </summary>
    public void Dispose()
    {
        httpClient?.Dispose();
    }
}