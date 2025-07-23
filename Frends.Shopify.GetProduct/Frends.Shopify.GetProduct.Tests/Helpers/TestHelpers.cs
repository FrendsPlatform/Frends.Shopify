using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetProduct.Tests.Helpers
{
    /// <summary>
    /// Helpers methods for testing. Creates and Deletes a product.
    /// </summary>
    internal static class TestHelpers
    {
        /// <summary>
        /// Creates a product in Shopify used for testing.
        /// </summary>
        /// <param name="accessToken">Api access token.</param>
        /// <param name="shopName">Shopify shop name.</param>
        /// <param name="apiVersion">Api version.</param>
        /// <returns>Product Id used for testing update functionality.</returns>
        public static async Task<string> CreateTestProduct(string accessToken, string shopName, string apiVersion)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

            var productData = new JObject
            {
                ["product"] = new JObject
                {
                    ["title"] = "Test Product",
                    ["body_html"] = "<p>Test description</p>",
                    ["vendor"] = "Test Vendor",
                    ["product_type"] = "Test Type",
                },
            };

            var content = new StringContent(
                productData.ToString(),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                $"https://{shopName}.myshopify.com/admin/api/{apiVersion}/products.json",
                content,
                CancellationToken.None);

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to create test product: {response.StatusCode} - {responseContent}");
            }

            return responseJson["product"]["id"].ToString();
        }

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