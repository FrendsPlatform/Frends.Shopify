using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.CreateProduct.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.Shopify.CreateProduct.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? _shopName = "testName";
    private readonly string? _accessToken = "testToken";
    private readonly string _apiVersion = "2024-04";
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
                ["title"] = $"Test Product {Guid.NewGuid()}",
                ["body_html"] = "<p>Test description</p>",
                ["vendor"] = "Test Vendor",
                ["product_type"] = "Test Type",
            },
        };

        _options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task CreateProduct_SuccessTest()
    {
        var result = await Shopify.CreateProduct(_input, _connection, _options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.CreatedProduct);
    }

    [TestMethod]
    public async Task CreateProduct_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = _accessToken,
            ApiVersion = _apiVersion,
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
            ShopName = _shopName,
            AccessToken = null,
            ApiVersion = _apiVersion,
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
}