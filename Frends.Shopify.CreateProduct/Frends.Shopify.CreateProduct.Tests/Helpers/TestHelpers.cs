using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Shopify.CreateProduct.Tests.Helpers
{
    /// <summary>
    /// Helper methods for tests. Deletes a product after tests have run.
    /// </summary>
    internal static class TestHelpers
    {
        /// <summary>
        /// Deletes a product in Shopify (used for test cleanup).
        /// </summary>
        /// <param name="productId">Id of product to be deleted.</param>
        /// <param name="accessToken">Api access token.</param>
        /// <param name="shopName">Shopify shop name.</param>
        /// <param name="apiVersion">Api version.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static async Task DeleteTestProduct(string productId, string accessToken, string shopName, string apiVersion)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

            var response = await client.DeleteAsync(
                $"https://{shopName}.myshopify.com/admin/api/{apiVersion}/products/{productId}.json",
                CancellationToken.None);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete test product: {response.StatusCode} - {responseContent}");
            }
        }
    }
}