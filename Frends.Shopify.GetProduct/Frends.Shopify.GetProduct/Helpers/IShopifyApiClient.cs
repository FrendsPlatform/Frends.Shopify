using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetProduct.Helpers;

/// <summary>
/// Interface for Shopify API implementations.
/// </summary>
public interface IShopifyApiClient
{
    /// <summary>
    /// Gets a product from Shopify.
    /// </summary>
    /// <param name="productId">ID of the product to retrieve.</param>
    /// <param name="fields">Optional fields to include in response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product data as JToken.</returns>
    Task<JToken> GetProductAsync(string productId, string[] fields, CancellationToken cancellationToken);
}