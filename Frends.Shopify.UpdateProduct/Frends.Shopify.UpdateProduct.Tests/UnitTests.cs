using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.UpdateProduct.Definitions;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Shopify.UpdateProduct.Tests;

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
            ProductData = new JObject
            {
                ["title"] = "Updated Test Product",
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

    [Test]
    public async Task UpdateProduct_SuccessTest()
    {
        mockShopifyClient.Setup(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await Shopify.UpdateProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task UpdateProduct_SuccessWithVariantsTest()
    {
        var variantInput = new Input
        {
            ProductId = "12345",
            ProductData = new JObject
            {
                ["title"] = "Updated Variant Test Product",
                ["variants"] = new JArray
                {
                    new JObject
                    {
                        ["option1"] = "Updated Size",
                        ["price"] = "29.99",
                        ["sku"] = "UPDATED-SIZE",
                    },
                },
            },
        };

        mockShopifyClient.Setup(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await Shopify.UpdateProduct(variantInput, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void UpdateProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = "test-token",
            ApiVersion = "2024-04",
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
        mockShopifyClient.Verify(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void UpdateProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = null,
            ApiVersion = "2024-04",
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
        mockShopifyClient.Verify(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void UpdateProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = "test-token",
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
        mockShopifyClient.Verify(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
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
            Shopify.UpdateProduct(invalidInput, connection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ProductId is required"));
        mockShopifyClient.Verify(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void UpdateProduct_ProductDataValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = "12345",
            ProductData = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(invalidInput, connection, new Options(), CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ProductData is required"));
        mockShopifyClient.Verify(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task UpdateProduct_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        mockShopifyClient.Setup(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error occurred"));

        var result = await Shopify.UpdateProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }

    [Test]
    public async Task UpdateProduct_HttpClientThrows_ReturnsErrorResult()
    {
        options.ThrowErrorOnFailure = false;

        mockShopifyClient.Setup(x => x.UpdateProductAsync(It.IsAny<string>(), It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await Shopify.UpdateProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Network error"));
    }

    [Test]
    public void ShopifyApiClient_UpdateProductAsync_AfterDispose_ThrowsException()
    {
        var client = new Helpers.ShopifyApiClient(connection);
        client.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(() =>
            client.UpdateProductAsync(input.ProductId, input.ProductData, CancellationToken.None));
    }
}