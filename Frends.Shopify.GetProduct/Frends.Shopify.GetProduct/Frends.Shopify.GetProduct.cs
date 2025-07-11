using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetProduct.Definitions;
using Frends.Shopify.GetProduct.Helpers;

namespace Frends.Shopify.GetProduct;

/// <summary>
/// Task class.
/// </summary>
public static class Shopify
{
    /// <summary>
    /// Retrieves a product from Shopify
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Shopify-GetProduct)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <param name="client">Optional: Shopify API client instance (for testing)</param>
    /// <returns>Object { bool Success, object Product, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> GetProduct(
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
            var product = await client.GetProductAsync(input.ProductId, options.Fields, cancellationToken);
            return new Result(true, product);
        }
        catch (Exception ex)
        {
            if (options.ThrowErrorOnFailure)
            {
                throw;
            }

            return new Result(false, null, new Error
            {
                Message = string.IsNullOrEmpty(options.ErrorMessageOnFailure)
                    ? ex.Message
                    : options.ErrorMessageOnFailure,
                AdditionalInfo = ex,
            });
        }
    }
}