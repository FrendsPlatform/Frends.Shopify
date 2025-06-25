using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.CreateProduct.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.CreateProduct.Tests;

[TestClass]
public class UnitTests
{
    private readonly string _shopName = "testshop";
    private readonly string _accessToken = "testtoken";
    private readonly string _apiVersion = "2023-10";
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
            ProductData = new JObject
            {
                ["title"] = "Test Product",
                ["body_html"] = "<p>Test description</p>",
                ["vendor"] = "Test Vendor",
            },
        };

        _options = new Options
        {
            ThrowErrorOnFailure = false,
            ErrorMessageOnFailure = "Test error occurred",
        };
    }

    [TestMethod]
    public async Task CreateProduct_SuccessTest()
    {
        var input = new Input
        {
            ProductData = new JObject
            {
                ["title"] = "Test Product",
                ["body_html"] = "<p>Test product description</p>",
                ["vendor"] = "Test Vendor",
                ["product_type"] = "Test Type",
            },
        };

        var options = new Options
        {
            ThrowErrorOnFailure = true,
        };

        var result = await Shopify.CreateProduct(input, _connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.CreatedProduct);
    }

    [TestMethod]
    public async Task CreateProduct_DuplicateProductTest()
    {
        var input = new Input
        {
            ProductData = new JObject
            {
                ["title"] = "Duplicate Test Product",
                ["body_html"] = "<p>Duplicate test</p>",
            },
        };

        var result = await Shopify.CreateProduct(input, _connection, new Options(), CancellationToken.None);
        Assert.IsTrue(result.Success);

        var result2 = await Shopify.CreateProduct(input, _connection, new Options(), CancellationToken.None);

        Assert.IsFalse(result2.Success);
        StringAssert.Contains(result2.Error.Message, "Shopify API error");
    }

    [TestMethod]
    public async Task CreateProduct_ValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = _accessToken,
            ApiVersion = "2023-10",
        };

        var input = new Input
        {
            ProductData = new JObject
            {
                ["title"] = "Should Fail",
            },
        };

        var result = await Shopify.CreateProduct(input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "ShopName is required");
    }
}