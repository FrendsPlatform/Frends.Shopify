using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetProduct.Definitions;
using Frends.Shopify.GetProduct.Helpers;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Shopify.GetProduct.Tests;

/// <summary>
/// Test cases for Shopify GetProduct task.
/// </summary>
[TestFixture]
public class UnitTests
{
    private Mock<Helpers.IShopifyApiClient> mockShopifyClient;
    private Connection connection;
    private Input input;
    private Options options;

    [SetUp]
    public void Setup()
    {
        mockShopifyClient = new Mock<Helpers.IShopifyApiClient>();

        connection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = "test-token",
            ApiVersion = "2024-04",
        };

        input = new Input
        {
            ProductId = "12345",
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [Test]
    public async Task GetProduct_SuccessTest()
    {
        var mockProduct = JToken.FromObject(new { id = "12345", title = "Test Product" });
        mockShopifyClient.Setup(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockProduct);

        var result = await Shopify.GetProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Product, Is.EqualTo(mockProduct));
    }

    [Test]
    public async Task GetProduct_WithFields_SuccessTest()
    {
        var mockProduct = JToken.FromObject(new { id = "12345" });
        options.Fields = ["id", "title"];

        mockShopifyClient.Setup(x => x.GetProductAsync(It.IsAny<string>(), options.Fields, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockProduct);

        var result = await Shopify.GetProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Product, Is.EqualTo(mockProduct));
    }

    [Test]
    public void GetProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = "test-token",
            ApiVersion = "2024-04",
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            Shopify.GetProduct(input, invalidConnection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
        mockShopifyClient.Verify(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void GetProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = null,
            ApiVersion = "2024-04",
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            Shopify.GetProduct(input, invalidConnection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
        mockShopifyClient.Verify(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void GetProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = "test-token",
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            Shopify.GetProduct(input, invalidConnection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
        mockShopifyClient.Verify(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void GetProduct_ProductIdValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = null,
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() =>
            Shopify.GetProduct(invalidInput, connection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ProductId is required"));
        mockShopifyClient.Verify(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void ShopifyApiClient_GetProductAsync_AfterDispose_ThrowsException()
    {
        var client = new Helpers.ShopifyApiClient(connection);
        client.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(() =>
            client.GetProductAsync(input.ProductId, null, CancellationToken.None));
    }

    [Test]
    public async Task GetProduct_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        mockShopifyClient.Setup(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error occurred"));

        var result = await Shopify.GetProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.EqualTo("Custom error message"));
        Assert.That(result.Error.AdditionalInfo.Message, Is.EqualTo("API error occurred"));
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