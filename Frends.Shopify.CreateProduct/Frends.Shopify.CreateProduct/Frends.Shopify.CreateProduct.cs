using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.CreateProduct.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.CreateProduct;

/// <summary>
/// Task class.
/// </summary>
public static class Shopify
{
    /// <summary>
    /// Creates a product in Shopify
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Shopify-CreateProduct)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>Object { bool Success, object CreatedProduct, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> CreateProduct(
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

            if (input.ProductData == null)
                throw new Exception("ProductData is required");

            var payload = new JObject
            {
                ["product"] = (JToken)input.ProductData,
            };

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);

            var content = new StringContent(
                payload.ToString(),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                $"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/products.json",
                content,
                cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseJson = JObject.Parse(responseContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = responseJson["errors"]?.ToString() ?? "Unknown error";
                throw new Exception($"Shopify API error: {response.StatusCode} - {error}");
            }

            return new Result(true, responseJson["product"] as JObject);
        }
        catch (Exception ex)
        {
            return Helpers.ErrorHandler.Handle(ex, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }
}