using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetOrders.Definitions;
using Frends.Shopify.GetOrders.Helpers;

namespace Frends.Shopify.GetOrders;

/// <summary>
/// Task class.
/// </summary>
public static class Shopify
{
    /// <summary>
    /// Retrieves orders from Shopify
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Shopify-GetOrders)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <param name="client">Optional: Shopify API client instance (for testing)</param>
    /// <returns>Object { bool Success, List&lt;object&gt; Orders, PageInfo PageInfo, Error Error }</returns>
    public static async Task<Result> GetOrders(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken,
        IShopifyApiClient client = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.ShopDomain))
                throw new Exception("ShopDomain is required");

            if (string.IsNullOrWhiteSpace(connection.AccessToken))
                throw new Exception("AccessToken is required");

            client ??= new ShopifyApiClient(connection);
            var response = await client.GetOrdersAsync(input.CreatedAtMin, input.CreatedAtMax, input.Status, input.FulfillmentStatus, options.Fields, options.Limit, options.PageInfo, cancellationToken);

            var ordersList = response.Orders?.ToObject<List<object>>();

            return new Result(true, ordersList, response.PageInfo);
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }
}