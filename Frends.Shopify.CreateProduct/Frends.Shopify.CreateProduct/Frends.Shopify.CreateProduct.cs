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
    /// Error handling
    /// </summary>
    private static class ErrorHandler
    {
        internal static Result Handle(Exception ex, bool throwError, string customMessage)
        {
            var error = new Error
            {
                Message = $"{customMessage} {ex.Message}",
                AdditionalInfo = ex,
            };

            if (throwError)
                throw new Exception(error.Message, ex);

            return new Result(false, null, error);
        }
    }

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
                throw new ArgumentException("ShopName is required", nameof(connection.ShopName));

            if (string.IsNullOrWhiteSpace(connection.AccessToken))
                throw new ArgumentException("AccessToken is required", nameof(connection.AccessToken));

            if (string.IsNullOrWhiteSpace(connection.ApiVersion))
                throw new ArgumentException("ApiVersion is required", nameof(connection.ApiVersion));

            if (input.ProductData == null)
                throw new ArgumentException("ProductData is required", nameof(input.ProductData));

            var payload = new JObject
            {
                ["product"] = (JToken)input.ProductData,
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);

                var content = new StringContent(
                    payload.ToString(),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(
                    $"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/products.json",
                    content,
                    cancellationToken);

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JObject.Parse(responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    var error = responseJson["errors"]?.ToString() ?? "Unknown error";
                    throw new Exception($"Shopify API error: {response.StatusCode} - {error}");
                }

                return new Result(true, responseJson["product"] as JObject);
            }
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(
                ex,
                options.ThrowErrorOnFailure,
                options.ErrorMessageOnFailure);
        }
    }
}