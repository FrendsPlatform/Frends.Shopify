using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.UpdateProduct.Definitions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Shopify.UpdateProduct.Tests;

/// <summary>
/// Test cases for Shopify UpdateProduct task.
/// </summary>
[TestFixture]
public class UnitTests
{
    private readonly string shopName = "frendstemplates";
    private readonly string accessToken;
    private readonly string apiVersion = "2025-07";
    private string productId;
    private Connection connection;
    private Input input;
    private Options options;

    public UnitTests()
    {
        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        accessToken = Environment.GetEnvironmentVariable("FRENDS_ShopifyTest_accessToken");
    }

    [SetUp]
    public void Setup()
    {
        connection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        input = new Input
        {
            ProductData = new JObject
            {
                ["title"] = $"Updated Test Product",
                ["body_html"] = "<p>Updated test description</p>",
                ["vendor"] = "Updated Test Vendor",
                ["product_type"] = "Updated Test Type",
            },
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TearDown]
    public async Task Cleanup()
    {
        if (!string.IsNullOrEmpty(productId))
        {
            try
            {
                await Helpers.TestHelpers.DeleteTestProduct(productId, accessToken, shopName, apiVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting test product: {ex.Message}");
            }
            finally
            {
                productId = null;
            }
        }
    }

    [Test]
    public async Task UpdateProduct_SuccessTest()
    {
        productId = await Helpers.TestHelpers.CreateTestProduct(accessToken, shopName, apiVersion);

        input.ProductId = productId;

        var result = await Shopify.UpdateProduct(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

        var response = await client.GetAsync(
            $"https://{shopName}.myshopify.com/admin/api/{apiVersion}/products/{productId}.json",
            CancellationToken.None);

        var responseContent = await response.Content.ReadAsStringAsync();
        var updatedProduct = JObject.Parse(responseContent)["product"];

        Assert.That(updatedProduct["title"]?.ToString(), Is.EqualTo(input.ProductData["title"]?.ToString()));
        Assert.That(updatedProduct["body_html"]?.ToString(), Is.EqualTo(input.ProductData["body_html"]?.ToString()));
        Assert.That(updatedProduct["vendor"]?.ToString(), Is.EqualTo(input.ProductData["vendor"]?.ToString()));
        Assert.That(updatedProduct["product_type"]?.ToString(), Is.EqualTo(input.ProductData["product_type"]?.ToString()));
    }

    [Test]
    public async Task UpdateProduct_WithVariants_SuccessTest()
    {
        productId = await Helpers.TestHelpers.CreateTestProduct(accessToken, shopName, apiVersion);

        var variantInput = new Input
        {
            ProductId = productId,
            ProductData = new JObject
            {
                ["title"] = "Updated Variant Test Product",
                ["body_html"] = "<p>Updated variant test description</p>",
                ["vendor"] = "Updated Variant Test Vendor",
                ["product_type"] = "Updated Variant Test Type",
                ["variants"] = new JArray
                {
                    new JObject
                    {
                        ["option1"] = "Updated Variant Size",
                        ["price"] = "29.99",
                        ["sku"] = "UPDATED-VARIANT-SIZE",
                    },
                },
            },
        };

        var result = await Shopify.UpdateProduct(variantInput, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

        var response = await client.GetAsync(
            $"https://{shopName}.myshopify.com/admin/api/{apiVersion}/products/{productId}.json",
            CancellationToken.None);

        var responseContent = await response.Content.ReadAsStringAsync();
        var updatedProduct = JObject.Parse(responseContent)["product"];
        var variants = updatedProduct["variants"] as JArray;

        Assert.That(updatedProduct["title"]?.ToString(), Is.EqualTo(variantInput.ProductData["title"].ToString()));
        Assert.That(updatedProduct["body_html"]?.ToString(), Is.EqualTo(variantInput.ProductData["body_html"].ToString()));
        Assert.That(updatedProduct["vendor"]?.ToString(), Is.EqualTo(variantInput.ProductData["vendor"].ToString()));
        Assert.That(updatedProduct["product_type"]?.ToString(), Is.EqualTo(variantInput.ProductData["product_type"].ToString()));

        Assert.That(variants, Is.Not.Null.And.Not.Empty);
        Assert.That(variants[0]["option1"]?.ToString(), Is.EqualTo("Updated Variant Size"));
        Assert.That(variants[0]["price"]?.ToString(), Is.EqualTo("29.99"));
        Assert.That(variants[0]["sku"]?.ToString(), Is.EqualTo("UPDATED-VARIANT-SIZE"));
    }

    [Test]
    public void UpdateProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
    }

    [Test]
    public void UpdateProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
    }

    [Test]
    public void UpdateProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
    }

    [Test]
    public void UpdateProduct_ProductIdValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = null,
            ProductData = new JObject
            {
                ["title"] = "Should Fail",
            },
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(invalidInput, connection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ProductId is required"));
    }

    [Test]
    public void UpdateProduct_ProductDataValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = "1234567890123",
            ProductData = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(invalidInput, connection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ProductData is required"));
    }

    [Test]
    public async Task UpdateProduct_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        var invalidInput = new Input
        {
            ProductId = "999999999999999999",
            ProductData = new JObject
            {
                ["title"] = "This should fail",
            },
        };

        var result = await Shopify.UpdateProduct(invalidInput, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }
}