using System;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.GetProduct.Definitions;
using Frends.Shopify.GetProduct.Helpers;
using NUnit.Framework;

namespace Frends.Shopify.GetProduct.Tests;

/// <summary>
/// Test cases for Shopify GetProduct task.
/// </summary>
[TestFixture]
public class UnitTests
{
    private readonly string shopName = "frendstemplates";
    private readonly string accessToken;
    private readonly string apiVersion = "2025-07";
    private readonly string productId = "7343567634535";
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
    public async Task GetProduct_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.GetProduct(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Product, Is.Not.Null);
        Assert.That(result.Product["id"]?.ToString(), Is.EqualTo(productId));
    }

    [Test]
    public async Task GetProduct_WithFields_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        options.Fields = ["id", "title", "vendor"];

        var result = await Shopify.GetProduct(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Product, Is.Not.Null);
        Assert.That(result.Product["id"]?.ToString(), Is.EqualTo(productId));
        Assert.That(result.Product["title"], Is.Not.Null);
        Assert.That(result.Product["vendor"], Is.Not.Null);
        Assert.That(result.Product["body_html"], Is.Null);
    }

    [Test]
    public void GetProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
    }

    [Test]
    public void GetProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
    }

    [Test]
    public void GetProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
    }

    [Test]
    public void GetProduct_ProductIdValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetProduct(invalidInput, connection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ProductId is required"));
    }

    [Test]
    public async Task GetProduct_ErrorHandlingTest()
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

        var result = await Shopify.GetProduct(invalidInput, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }

    [Test]
    public void GetProduct_Handle_WhenThrowErrorIsTrue_ThrowsException()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error context";

        var thrownEx = Assert.Throws<Exception>(() =>
            ErrorHandler.Handle(ex, true, customMessage));

        Assert.That(thrownEx.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(thrownEx.InnerException, Is.EqualTo(ex));
    }

    [Test]
    public void GetProduct_Handle_WhenThrowErrorIsFalse_ReturnsErrorResult()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error context";

        var result = ErrorHandler.Handle(ex, false, customMessage);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Product, Is.Null);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(result.Error.AdditionalInfo, Is.EqualTo(ex));
    }
}