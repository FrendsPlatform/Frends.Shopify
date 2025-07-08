using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.DeleteProduct.Definitions;
using Frends.Shopify.DeleteProduct.Helpers;

namespace Frends.Shopify.DeleteProduct;

/// <summary>
/// Task class.
/// </summary>
public static class Shopify
{
    /// <summary>
    /// Deletes a product in Shopify
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Shopify-DeleteProduct)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <param name="client">Optional: Shopify API client instance (for testing)</param>
    /// <returns>Object { bool Success, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> DeleteProduct(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken,
        IShopifyApiClient client = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.ShopName))
                throw new ArgumentException("ShopName is required");

            if (string.IsNullOrWhiteSpace(connection.AccessToken))
                throw new ArgumentException("AccessToken is required");

            if (string.IsNullOrWhiteSpace(connection.ApiVersion))
                throw new ArgumentException("ApiVersion is required");

            if (string.IsNullOrWhiteSpace(input.ProductId))
                throw new ArgumentException("ProductId is required");

            client ??= new ShopifyApiClient(connection);

            await client.DeleteProductAsync(input.ProductId, cancellationToken);
            return new Result(true);
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }
}