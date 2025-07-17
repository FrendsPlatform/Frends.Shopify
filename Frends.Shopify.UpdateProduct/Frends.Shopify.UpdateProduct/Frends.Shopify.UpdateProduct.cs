using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.UpdateProduct.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.UpdateProduct;

/// <summary>
/// Task class.
/// </summary>
public static class Shopify
{
    /// <summary>
    /// Updates a product in Shopify
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Shopify-UpdateProduct)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>Object { bool Success, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> UpdateProduct(
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

            if (input.ProductData == null)
                throw new Exception("ProductData is required");

            var payload = new JObject
            {
                ["product"] = JToken.FromObject(input.ProductData),
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);

            var content = new StringContent(
                payload.ToString(),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync(
                $"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/products/{input.ProductId}.json",
                content,
                cancellationToken);

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

            return new Result(true);
        }
        catch (Exception ex)
        {
            return Helpers.ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }
}