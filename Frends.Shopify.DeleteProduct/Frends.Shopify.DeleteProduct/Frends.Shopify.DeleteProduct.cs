using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.DeleteProduct.Definitions;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.DeleteProduct;

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

            return new Result(false, error);
        }
    }

    /// <summary>
    /// Deletes a product in Shopify
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Shopify-DeleteProduct)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>Object { bool Success, Error Error { string Message, Exception AdditionalInfo } }</returns>
    public static async Task<Result> DeleteProduct(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
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

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", connection.AccessToken);

            var response = await client.DeleteAsync(
                $"https://{connection.ShopName}.myshopify.com/admin/api/{connection.ApiVersion}/products/{input.ProductId}.json",
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
            return ErrorHandler.Handle(
                ex,
                options.ThrowErrorOnFailure,
                options.ErrorMessageOnFailure);
        }
    }
}