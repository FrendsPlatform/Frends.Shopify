using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.CreateProduct.Helpers;

/// <summary>
/// Interface for Shopify API implementations.
/// </summary>
public interface IShopifyApiClient
{
    /// <summary>
    /// Creates a new product in Shopify.
    /// </summary>
    /// <param name="productData">Product data as JObject.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>JObject containing the Shopify API response with created product details.</returns>
    Task<JObject> CreateProductAsync(JObject productData, CancellationToken cancellationToken);
}