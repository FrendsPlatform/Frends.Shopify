using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetProduct.Definitions;
using Newtonsoft.Json.Linq;

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
    /// <returns>Object { bool Success, JObject Product, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> GetProduct(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.ShopName))
                throw new Exception("ShopName is required");

            if (string.IsNullOrWhiteSpace(connection.AccessToken))
                throw new Exception("AccessToken is required");

            if (string.IsNullOrWhiteSpace(connection.ApiVersion))
                throw new Exception("ApiVersion is required");

            if (string.IsNullOrWhiteSpace(input.ProductId))
                throw new Exception("ProductId is required");

            var url = $"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/products/{input.ProductId}.json";

            if (options.Fields != null && options.Fields.Length > 0)
            {
                var fields = string.Join(",", options.Fields);
                url += $"?fields={fields}";
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);

            var response = await client.GetAsync(url, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseJson = JObject.Parse(responseContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = responseJson["errors"]?.ToString() ?? responseContent;

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"Product with ID '{input.ProductId}' was not found.");
                }
                else if (error.Contains("expected String to be a id"))
                {
                    throw new Exception($"Invalid Product ID format: '{input.ProductId}'. Product ID should be a valid numeric value.");
                }
                else
                {
                    throw new Exception($"Shopify API error: {response.StatusCode} - {error}");
                }
            }

            return new Result(true, responseJson["product"] as JObject);
        }
        catch (Exception ex)
        {
            return Helpers.ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, string.IsNullOrEmpty(options.ErrorMessageOnFailure) ? "Failed to get product:" : options.ErrorMessageOnFailure);
        }
    }
}