using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.DeleteProduct.Definitions;

namespace Frends.Shopify.DeleteProduct.Helpers;

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
    /// Deletes an existing product in Shopify.
    /// </summary>
    /// <param name="productId">The ID of the product to delete. Must be a valid Shopify product ID.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>A Task representing the asynchronous delete operation.</returns>
    public async Task DeleteProductAsync(string productId, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"products/{productId}.json", cancellationToken);
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