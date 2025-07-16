using System.Threading;
using System.Threading.Tasks;

namespace Frends.Shopify.DeleteProduct.Helpers;

/// <summary>
/// Interface for Shopify API implementations.
/// </summary>
public interface IShopifyApiClient
{
    /// <summary>
    /// Deletes a product in Shopify.
    /// </summary>
    /// <param name="productId">ID of the product to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task DeleteProductAsync(string productId, CancellationToken cancellationToken);
}