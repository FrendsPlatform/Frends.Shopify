using System;
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
/// <example>
/// <code>
/// // Example test product with variants:
/// var input = new Input
/// {
///     ProductId = "12345",
///     ProductData = new JObject
///     {
///         ["title"] = "Updated Test Product",
///         ["variants"] = new JArray
///         {
///             new JObject
///             {
///                 ["option1"] = "Updated Size",
///                 ["price"] = "39.99",
///                 ["sku"] = "UPDATED-SIZE"
///             }
///         }
///     }
/// };
/// </code>
/// </example>
[TestFixture]
public class UnitTests
{
    private readonly string shopName = "frendstemplates";
    private readonly string accessToken;
    private readonly string apiVersion = "2025-07";
    private readonly string productId = "7343388950631";
    private readonly string variantProductId = "7343388983399";
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

    [Test]
    public async Task UpdateProduct_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.UpdateProduct(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task UpdateProduct_WithVariants_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
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

        Assert.That(result.Success, Is.True);
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
            ProductId = productId,
            ProductData = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.UpdateProduct(invalidInput, connection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ProductData is required"));
    }

    [Test]
    public async Task UpdateProduct_ErrorHandlingTest()
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