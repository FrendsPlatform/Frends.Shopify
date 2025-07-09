using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.UpdateProduct.Helpers;

/// <summary>
/// Interface for Shopify API implementations.
/// </summary>
public interface IShopifyApiClient
{
    /// <summary>
    /// Updates a product in Shopify.
    /// </summary>
    /// <param name="productId">ID of the product to update. Must be a valid Shopify product ID.</param>
    /// <param name="productData">Product data as JObject containing the fields to update.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>A Task that represents the asynchronous update operation.</returns>
    Task UpdateProductAsync(string productId, JObject productData, CancellationToken cancellationToken);
}