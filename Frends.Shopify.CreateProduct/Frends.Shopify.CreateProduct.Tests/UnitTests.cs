using System;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.CreateProduct.Definitions;
using Frends.Shopify.CreateProduct.Helpers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Shopify.CreateProduct.Tests;

/// <summary>
/// Test cases for Shopify CreateProduct task.
/// </summary>
/// <example>
/// <code>
/// // Example test product with variants:
/// new JObject
/// {
///     ["title"] = "Test Product",
///     ["body_html"] = "<p>Test description</p>",
///     ["vendor"] = "Test Vendor",
///     ["product_type"] = "Test Type",
///     ["variants"] = new JArray
///     {
///         new JObject
///         {
///             ["option1"] = "Default",
///             ["price"] = "9.99",
///             ["sku"] = "PROD-001"
///         }
///     }
/// }
/// </code>
/// </example>
[TestFixture]
public class UnitTests
{
    private readonly string shopName = "frendstemplates";
    private readonly string accessToken;
    private readonly string apiVersion = "2025-07";
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
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.CreateProduct(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.CreatedProduct, Is.Not.Null);
        Assert.That(result.CreatedProduct["title"]?.ToString(), Is.EqualTo("Test Product"));
    }

    [Test]
    public async Task CreateProduct_WithVariants_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        input.ProductData["title"] = "Variant Test Product";
        input.ProductData["variants"] = new JArray
        {
            new JObject
            {
                ["option1"] = "Size",
                ["price"] = "10.99",
                ["sku"] = "TEST-SIZE",
            },
        };

        var result = await Shopify.CreateProduct(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.CreatedProduct["variants"], Is.Not.Null);
        Assert.That(result.CreatedProduct["variants"][0]["sku"]?.ToString(), Is.EqualTo("TEST-SIZE"));
    }

    [Test]
    public void CreateProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = "test-token",
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.CreateProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
    }

    [Test]
    public void CreateProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.CreateProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
    }

    [Test]
    public void CreateProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.CreateProduct(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
    }

    [Test]
    public void CreateProduct_ProductDataValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductData = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.CreateProduct(invalidInput, connection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ProductData is required"));
    }

    [Test]
    public async Task CreateProduct_ErrorHandlingTest()
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
            ProductData = new JObject
            {
                ["invalid_field"] = "This will cause an error",
            },
        };

        var result = await Shopify.CreateProduct(invalidInput, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }

    [Test]
    public void CreateProduct_Handle_WhenThrowErrorIsTrue_ThrowsException()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error context";

        var thrownEx = Assert.Throws<Exception>(() =>
            ErrorHandler.Handle(ex, true, customMessage));

        Assert.That(thrownEx.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(thrownEx.InnerException, Is.EqualTo(ex));
    }

    [Test]
    public void CreateProduct_Handle_WhenThrowErrorIsFalse_ReturnsErrorResult()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error context";

        var result = ErrorHandler.Handle(ex, false, customMessage);

        Assert.That(result.Success, Is.False);
        Assert.That(result.CreatedProduct, Is.Null);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(result.Error.AdditionalInfo, Is.EqualTo(ex));
    }
}