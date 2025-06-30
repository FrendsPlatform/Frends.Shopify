using System;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.UpdateProduct.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.UpdateProduct.Tests;

/// <summary>
/// Test cases for Shopify UpdateProduct task.
/// </summary>
/// <example>
/// <code>
/// // Example test product with variants:
/// var input = new Input
/// {
///     ProductId = "12345",
///     ProductData = new JObject
///     {
///         ["product"] = new JObject
///         {
///             ["title"] = "Updated Test Product",
///             ["variants"] = new JArray
///             {
///                 new JObject
///                 {
///                     ["option1"] = "Updated Size",
///                     ["price"] = "39.99",
///                     ["sku"] = "UPDATED-SIZE"
///                 }
///             }
///         }
///     }
/// };
/// </code>
/// </example>
[TestClass]
public class UnitTests
{
    private readonly string shopName;
    private readonly string accessToken;
    private readonly string apiVersion = "2024-04";
    private readonly string productId = "7323461845095";
    private readonly string variantProductId = "7323461910631";
    private Connection connection;
    private Input input;
    private Options options;

    public UnitTests()
    {
        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        shopName = Environment.GetEnvironmentVariable("FRENDS_ShopifyTest_shopName");
        accessToken = Environment.GetEnvironmentVariable("FRENDS_ShopifyTest_accessToken");
    }

    [TestInitialize]
    public void TestInitialize()
    {
        connection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        input = new Input
        {
            ProductId = productId,
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

    [TestMethod]
    public async Task UpdateProduct_SuccessTest()
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.UpdateProduct(input, connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task UpdateProduct_SuccessWithVariantsTest()
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var variantInput = new Input
        {
            ProductId = variantProductId,
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
        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task UpdateProduct_ShopNameValidationFailureTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var result = await Shopify.UpdateProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ShopName is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task UpdateProduct_AccessTokenValidationFailureTest()
    {
        if (string.IsNullOrEmpty(shopName))
        {
            Assert.Inconclusive("ShopName not configured in environment variables. Test skipped.");
            return;
        }

        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var result = await Shopify.UpdateProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("AccessToken is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task UpdateProduct_ApiVersionValidationFailureTest()
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var result = await Shopify.UpdateProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ApiVersion is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task UpdateProduct_ProductIdValidationFailureTest()
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var invalidInput = new Input
        {
            ProductId = null,
            ProductData = new JObject
            {
                ["title"] = "Should Fail",
            },
        };

        var result = await Shopify.UpdateProduct(
            invalidInput,
            connection,
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ProductId is required") ||
            result.Error.Message.Contains("ProductId (Parameter 'ProductId')"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task UpdateProduct_ProductDataValidationFailureTest()
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var invalidInput = new Input
        {
            ProductId = productId,
            ProductData = null,
        };

        var result = await Shopify.UpdateProduct(
            invalidInput,
            connection,
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ProductData is required") ||
            result.Error.Message.Contains("ProductData (Parameter 'ProductData')"),
            $"Actual error message: {result.Error.Message}");
    }
}