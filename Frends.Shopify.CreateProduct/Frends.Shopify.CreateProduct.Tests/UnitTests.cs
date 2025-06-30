using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.CreateProduct.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

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
///                     ["price"] = "29.99",
///                     ["sku"] = "TEST-SIZE"
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
    private readonly string shopName = "testName";
    private readonly string accessToken = "testToken";
    private readonly string apiVersion = "2024-04";
    private Connection connection;
    private Input input;
    private Options options;

    [TestInitialize]
    public void Init()
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
                ["title"] = $"Test Product",
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

    [TestMethod]
    public async Task CreateProduct_SuccessTest()
    {
        var result = await Shopify.CreateProduct(input, connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.CreatedProduct);
    }

    [TestMethod]
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

        var result = await Shopify.CreateProduct(variantInput, connection, options, CancellationToken.None);
        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public async Task CreateProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var input = new Input
        {
            ProductData = new JObject
            {
                ["title"] = "Should Fail",
                ["body_html"] = "<p>Should Fail</p>",
                ["vendor"] = "Should Fail",
                ["product_type"] = "Should Fail",
            },
        };

        var result = await Shopify.CreateProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "ShopName is required");
    }

    [TestMethod]
    public async Task CreateProduct_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var input = new Input
        {
            ProductData = new JObject
            {
                ["title"] = "Should Fail",
                ["body_html"] = "<p>Should Fail</p>",
                ["vendor"] = "Should Fail",
                ["product_type"] = "Should Fail",
            },
        };

        var result = await Shopify.CreateProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "AccessToken is required");
    }

    [TestMethod]
    public async Task CreateProduct_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var result = await Shopify.CreateProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "ApiVersion is required");
    }

    [TestMethod]
    public async Task CreateProduct_ProductDataValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductData = null,
        };

        var result = await Shopify.CreateProduct(invalidInput, connection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "ProductData is required");
    }

    [TestMethod]
    public async Task CreateProduct_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        var invalidInput = new Input
        {
            ProductData = new { Invalid = "data" },
        };

        var result = await Shopify.CreateProduct(invalidInput, connection, options, CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "Custom error message");
    }
}