using System;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.DeleteProduct.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Shopify.DeleteProduct.Tests;

[TestClass]
public class UnitTests
{
    private readonly string shopName;
    private readonly string accessToken;
    private readonly string apiVersion = "2024-04";
    private readonly string productId = "7323461845095";
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
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task DeleteProduct_SuccessTest()
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.DeleteProduct(input, connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task DeleteProduct_ShopNameValidationFailureTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var result = await Shopify.DeleteProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ShopName is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task DeleteProduct_AccessTokenValidationFailureTest()
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

        var result = await Shopify.DeleteProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("AccessToken is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task DeleteProduct_ApiVersionValidationFailureTest()
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

        var result = await Shopify.DeleteProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ApiVersion is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task DeleteProduct_ProductIdValidationFailureTest()
    {
        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(accessToken))
        {
            Assert.Inconclusive("ShopName or AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var invalidInput = new Input
        {
            ProductId = null,
        };

        var result = await Shopify.DeleteProduct(invalidInput, connection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ProductId is required") ||
            result.Error.Message.Contains("ProductId (Parameter 'ProductId')"),
            $"Actual error message: {result.Error.Message}");
    }
}