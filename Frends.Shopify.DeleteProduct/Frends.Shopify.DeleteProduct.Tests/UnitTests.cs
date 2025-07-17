using System;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.DeleteProduct.Definitions;
using NUnit.Framework;

namespace Frends.Shopify.DeleteProduct.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string shopName = "frendstemplates";
    private readonly string accessToken;
    private readonly string apiVersion = "2025-07";
    private readonly string productId = "7343390883943";
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
            ProductId = productId,
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [Test]
    public async Task DeleteProduct_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.DeleteProduct(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void DeleteProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
    }

    [Test]
    public void DeleteProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
    }

    [Test]
    public void DeleteProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
    }

    [Test]
    public void DeleteProduct_ProductIdValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(invalidInput, connection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ProductId is required"));
    }

    [Test]
    public async Task DeleteProduct_ErrorHandlingTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        var invalidInput = new Input
        {
            ProductId = "999999999999999999",
        };

        var result = await Shopify.DeleteProduct(invalidInput, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }
}