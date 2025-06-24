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
}