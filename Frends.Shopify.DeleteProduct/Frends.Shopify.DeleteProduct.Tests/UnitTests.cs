using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.DeleteProduct.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Shopify.DeleteProduct.Tests;

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
        };

        _options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task UpdateProduct_SuccessTest()
    {
        var result = await Shopify.DeleteProduct(_input, _connection, _options, CancellationToken.None);

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

        var result = await Shopify.DeleteProduct(_input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "ShopName is required");
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

        var result = await Shopify.DeleteProduct(_input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "AccessToken is required");
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

        var result = await Shopify.DeleteProduct(_input, invalidConnection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "ApiVersion is required");
    }

    [TestMethod]
    public async Task UpdateProduct_ProductIdValidationFailureTest()
    {
        var invalidInput = new Input
        {
            ProductId = null,
        };

        var result = await Shopify.DeleteProduct(invalidInput, _connection, new Options(), CancellationToken.None);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error.Message, "ProductId is required");
    }
}