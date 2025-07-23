using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetOrders.Definitions;
using Newtonsoft.Json.Linq;

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
    /// <returns>Object { bool Success, List Orders, PageInfo PageInfo, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> GetOrders(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.ShopDomain))
                throw new Exception("ShopDomain is required");

            if (string.IsNullOrWhiteSpace(connection.AccessToken))
                throw new Exception("AccessToken is required");

            if (string.IsNullOrWhiteSpace(connection.ApiVersion))
                throw new Exception("ApiVersion is required");

            var url = $"https://{connection.ShopDomain}/admin/api/{connection.ApiVersion}/orders.json";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(input.CreatedAtMin))
                queryParams.Add($"created_at_min={Uri.EscapeDataString(input.CreatedAtMin)}");

            if (!string.IsNullOrEmpty(input.CreatedAtMax))
                queryParams.Add($"created_at_max={Uri.EscapeDataString(input.CreatedAtMax)}");

            if (!string.IsNullOrEmpty(input.Status) && input.Status != "any")
                queryParams.Add($"status={input.Status}");

            if (!string.IsNullOrEmpty(input.FulfillmentStatus))
                queryParams.Add($"fulfillment_status={input.FulfillmentStatus}");

            if (!string.IsNullOrEmpty(options.Fields))
                queryParams.Add($"fields={Uri.EscapeDataString(options.Fields)}");

            if (options.Limit > 0)
                queryParams.Add($"limit={options.Limit}");

            if (!string.IsNullOrEmpty(options.PageInfo))
                queryParams.Add($"page_info={Uri.EscapeDataString(options.PageInfo)}");

            if (queryParams.Count > 0)
                url += $"?{string.Join("&", queryParams)}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);

            var response = await client.GetAsync(url, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseJson = JObject.Parse(responseContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = responseJson["errors"]?.ToString() ?? responseContent;
                throw new Exception($"Shopify API error: {response.StatusCode} - {error}");
            }

            var pageInfo = Helpers.HeaderParser.ParseLinkHeader(response.Headers);
            var ordersArray = responseJson["orders"] as JArray;
            var ordersList = ordersArray?.ToObject<List<object>>() ?? [];

            return new Result(true, ordersList, pageInfo);
        }
        catch (Exception ex)
        {
            return Helpers.ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, string.IsNullOrEmpty(options.ErrorMessageOnFailure) ? "Failed to get orders:" : options.ErrorMessageOnFailure);
        }
    }
}