using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.DeleteProduct.Definitions;
using Moq;
using NUnit.Framework;

namespace Frends.Shopify.DeleteProduct.Tests;

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
    public async Task DeleteProduct_SuccessTest()
    {
        mockShopifyClient.Setup(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await Shopify.DeleteProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void DeleteProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = "test-token",
            ApiVersion = "2024-04",
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
        mockShopifyClient.Verify(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void DeleteProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = null,
            ApiVersion = "2024-04",
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
        mockShopifyClient.Verify(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void DeleteProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = "test-token",
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
        mockShopifyClient.Verify(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void DeleteProduct_ProductIdValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.DeleteProduct(invalidInput, connection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ProductId is required"));
        mockShopifyClient.Verify(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteProduct_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        mockShopifyClient.Setup(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error occurred"));

        var result = await Shopify.DeleteProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }

    [Test]
    public async Task DeleteProduct_HttpClientThrows_ReturnsErrorResult()
    {
        options.ThrowErrorOnFailure = false;

        mockShopifyClient.Setup(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await Shopify.DeleteProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Network error"));
    }

    [Test]
    public void ShopifyApiClient_DeleteProductAsync_AfterDispose_ThrowsException()
    {
        var client = new Helpers.ShopifyApiClient(connection);
        client.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(() =>
            client.DeleteProductAsync(input.ProductId, CancellationToken.None));
    }

    [Test]
    public async Task DeleteProduct_NotFoundError_ReturnsAppropriateMessage()
    {
        options.ThrowErrorOnFailure = false;

        mockShopifyClient.Setup(x => x.DeleteProductAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Not Found", null, System.Net.HttpStatusCode.NotFound));

        var result = await Shopify.DeleteProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Not Found"));
    }
}