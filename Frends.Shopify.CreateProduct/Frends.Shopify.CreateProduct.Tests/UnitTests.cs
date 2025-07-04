using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.CreateProduct.Definitions;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Frends.Shopify.CreateProduct.Tests;

/// <summary>
/// Test cases for Shopify CreateProduct task.
/// </summary>
/// <example>
/// <code>
/// // Example test product with variants:
/// var input = new Input
/// {
///     ProductData = new JObject
///     {
///         ["product"] = new JObject
///         {
///             ["title"] = "Test Product",
///             ["variants"] = new JArray
///             {
///                 new JObject
///                 {
///                     ["option1"] = "Size",
///                     ["price"] = "10.99",
///                     ["sku"] = "TEST-SIZE"
///                 }
///             }
///         }
///     }
/// };
/// </code>
/// </example>
[TestFixture]
public class UnitTests
{
    private Mock<Helpers.IShopifyApiClient> mockShopifyClient;
    private Connection connection;
    private Input input;
    private Options options;
    private MockHttpMessageHandler mockHttpHandler;

    [SetUp]
    public void Setup()
    {
        mockShopifyClient = new Mock<Helpers.IShopifyApiClient>();
        mockHttpHandler = new MockHttpMessageHandler();

        connection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = "test-token",
            ApiVersion = "2024-04",
        };

        input = new Input
        {
            ProductData = new JObject
            {
                ["title"] = "Test Product",
                ["body_html"] = "<p>Test description</p>",
                ["vendor"] = "Test Vendor",
                ["product_type"] = "Test Type",
            },
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [Test]
    public async Task CreateProduct_SuccessTest()
    {
        var mockResponse = new JObject
        {
            ["product"] = new JObject
            {
                ["id"] = 12345,
                ["title"] = "Test Product",
            },
        };

        mockShopifyClient.Setup(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var result = await Shopify.CreateProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.CreatedProduct, Is.Not.Null);
    }

    [Test]
    public async Task CreateProduct_SuccessWithVariantsTest()
    {
        var variantInput = new Input
        {
            ProductData = new JObject
            {
                ["title"] = "Variant Test Product",
                ["variants"] = new JArray
                {
                    new JObject
                    {
                        ["option1"] = "Size",
                        ["price"] = "10.99",
                        ["sku"] = "TEST-SIZE",
                    },
                },
            },
        };

        var mockResponse = new JObject
        {
            ["product"] = new JObject
            {
                ["id"] = 12345,
                ["title"] = "Variant Test Product",
                ["variants"] = new JArray
                {
                    new JObject
                    {
                        ["id"] = 67890,
                        ["option1"] = "Size",
                        ["price"] = "10.99",
                        ["sku"] = "TEST-SIZE",
                    },
                },
            },
        };

        mockShopifyClient.Setup(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var result = await Shopify.CreateProduct(variantInput, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task CreateProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = "test-token",
            ApiVersion = "2024-04",
        };

        var result = await Shopify.CreateProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("ShopName is required"));
        mockShopifyClient.Verify(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = null,
            ApiVersion = "2024-04",
        };

        var result = await Shopify.CreateProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("AccessToken is required"));
        mockShopifyClient.Verify(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = "test-shop",
            AccessToken = "test-token",
            ApiVersion = null,
        };

        var result = await Shopify.CreateProduct(input, invalidConnection, new Options(), CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("ApiVersion is required"));
        mockShopifyClient.Verify(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateProduct_ProductDataValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductData = null,
        };

        var result = await Shopify.CreateProduct(invalidInput, connection, new Options(), CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("ProductData is required"));
        mockShopifyClient.Verify(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateProduct_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        var invalidInput = new Input
        {
            ProductData = new JObject
            {
                ["Invalid"] = "data",
            },
        };

        mockShopifyClient.Setup(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error occurred"));

        var result = await Shopify.CreateProduct(invalidInput, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }

    [Test]
    public async Task CreateProduct_HttpClientThrows_ReturnsErrorResult()
    {
        options.ThrowErrorOnFailure = false;

        mockShopifyClient.Setup(x => x.CreateProductAsync(It.IsAny<JObject>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await Shopify.CreateProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Network error"));
    }

    [Test]
    public async Task CreateProduct_MissingRequiredFields_ReturnsError()
    {
        options.ThrowErrorOnFailure = false;

        input.ProductData = new JObject
        {
            ["body_html"] = "<p>Test</p>",
        };

        mockShopifyClient.Setup(x => x.CreateProductAsync(It.Is<JObject>(j => j["title"] == null), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Title can't be blank"));

        var result = await Shopify.CreateProduct(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Title can't be blank"));
    }

    [Test]
    public void RealShopifyApiClient_CreateProductAsync_AfterDispose_ThrowsException()
    {
        var client = new Helpers.RealShopifyApiClient(connection);
        client.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(() =>
            client.CreateProductAsync(input.ProductData, CancellationToken.None));
    }
}