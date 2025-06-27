using System.Threading;
using System.Threading.Tasks;
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
    private readonly string _shopName = "testName";
    private readonly string _accessToken = "testToken";
    private readonly string _apiVersion = "2024-04";
    private readonly string _productId = "testId";
    private Connection _connection;
    private Input _input;
    private Options _options;

    [TestInitialize]
    public void TestInitialize()
    {
        _connection = new Connection
        {
            ShopName = _shopName,
            AccessToken = _accessToken,
            ApiVersion = _apiVersion,
        };

        _input = new Input
        {
            ProductId = _productId,
            ProductData = new JObject
            {
                ["title"] = $"Updated Test Product",
                ["body_html"] = "<p>Updated test description</p>",
                ["vendor"] = "Updated Test Vendor",
                ["product_type"] = "Updated Test Type",
            },
        };

        _options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task UpdateProduct_SuccessTest()
    {
        var result = await Shopify.UpdateProduct(_input, _connection, _options, CancellationToken.None);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task UpdateProduct_SuccessWithVariantsTest()
    {
        var variantInput = new Input
        {
            ProductId = _productId,
            ProductData = new JObject
            {
                ["title"] = "Updated Variant Test Product",
                ["variants"] = new JArray
                {
                    new JObject
                    {
                        ["option1"] = "Updated Size",
                        ["price"] = "39.99",
                        ["sku"] = "UPDATED-SIZE",
                    },
                },
            },
        };

        var result = await Shopify.UpdateProduct(variantInput, _connection, _options, CancellationToken.None);
        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task UpdateProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = _accessToken,
            ApiVersion = _apiVersion,
        };

        var result = await Shopify.UpdateProduct(_input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ShopName is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task UpdateProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = _shopName,
            AccessToken = null,
            ApiVersion = _apiVersion,
        };

        var result = await Shopify.UpdateProduct(_input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("AccessToken is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task UpdateProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = _shopName,
            AccessToken = _accessToken,
            ApiVersion = null,
        };

        var result = await Shopify.UpdateProduct(_input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ApiVersion is required"),
            $"Actual error message: {result.Error.Message}");
    }

    [TestMethod]
    public async Task UpdateProduct_ProductIdValidationFailureTest()
    {
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
            _connection,
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
        var invalidInput = new Input
        {
            ProductId = _productId,
            ProductData = null,
        };

        var result = await Shopify.UpdateProduct(
            invalidInput,
            _connection,
            new Options { ThrowErrorOnFailure = false },
            CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsTrue(
            result.Error.Message.Contains("ProductData is required") ||
            result.Error.Message.Contains("ProductData (Parameter 'ProductData')"),
            $"Actual error message: {result.Error.Message}");
    }
}