using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.GetOrders.Definitions;
using Frends.Shopify.GetOrders.Helpers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Shopify.GetOrders.Tests;

[TestFixture]
public class UnitTests
{
    private readonly string shopDomain = "frendstemplates.myshopify.com";
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
            ShopDomain = shopDomain,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        input = new Input
        {
            CreatedAtMin = "2023-01-01T00:00:00Z",
            CreatedAtMax = "2025-07-10T07:07:32Z",
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
            Limit = 5,
        };
    }

    [Test]
    public async Task GetOrders_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None);

        Console.WriteLine($"Retrieved {result.Orders?.Count ?? 0} orders.");
        if (result.Orders != null)
        {
            foreach (var order in result.Orders)
            {
                Console.WriteLine(JObject.FromObject(order).ToString());
            }
        }

        if (result.Orders.Count == 0)
            Assert.Ignore("Test skipped. Did not retrieve any orders.");

        Assert.That(result.Success, Is.True);
        Assert.That(result.Orders, Is.Not.Null);
        Assert.That(result.Orders.Count, Is.LessThanOrEqualTo(options.Limit));
    }

    [Test]
    public async Task GetOrders_WithFields_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        options.Fields = "id,created_at,total_price";

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None);

        Console.WriteLine($"Retrieved {result.Orders?.Count ?? 0} orders.");
        if (result.Orders != null)
        {
            foreach (var order in result.Orders)
            {
                Console.WriteLine(JObject.FromObject(order).ToString());
            }
        }

        if (result.Orders.Count == 0)
            Assert.Ignore("Test skipped. Did not retrieve any orders.");

        Assert.That(result.Success, Is.True);
        Assert.That(result.Orders, Is.Not.Null);
    }

    [Test]
    public async Task GetOrders_WithStatusFilter_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        input.Status = "cancelled";
        options.Fields = "id,created_at,total_price,cancelled_at";

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None);

        Console.WriteLine($"Retrieved {result.Orders?.Count ?? 0} orders.");

        if (result.Orders != null && result.Orders.Count > 0)
        {
            foreach (var order in result.Orders)
            {
                var orderJson = JObject.FromObject(order);
                Console.WriteLine(orderJson.ToString());

                Assert.That(orderJson["cancelled_at"].Type == JTokenType.Null, Is.False, $"Order {orderJson["id"]} has null cancelled_at. Returned order is not of type cancelled");
            }
        }
        else
        {
            if (result.Orders.Count == 0)
                Assert.Ignore("Test skipped. Did not retrieve any orders.");
        }

        Assert.That(result.Success, Is.True);
        Assert.That(result.Orders, Is.Not.Null);
    }

    [Test]
    public void GetOrders_ShopDomainValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopDomain = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetOrders(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ShopDomain is required"));
    }

    [Test]
    public void GetOrders_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopDomain = shopDomain,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetOrders(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
    }

    [Test]
    public void GetOrders_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopDomain = shopDomain,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetOrders(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
    }

    [Test]
    public async Task GetOrders_ErrorHandlingTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        input.CreatedAtMin = "invalid-date";

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }

    [Test]
    public void ParseLinkHeader_WithNextPage_ReturnsCorrectPageInfo()
    {
        var headers = new HttpResponseMessage().Headers;
        headers.Add("Link", "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=nextPageCursor123>; rel=\"next\"");

        var result = HeaderParser.ParseLinkHeader(headers);

        Assert.That(result.NextPage, Is.EqualTo("nextPageCursor123"));
        Assert.That(result.PreviousPage, Is.Null);
    }

    [Test]
    public void ParseLinkHeader_WithPreviousPage_ReturnsCorrectPageInfo()
    {
        var headers = new HttpResponseMessage().Headers;
        headers.Add("Link", "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=prevPageCursor456>; rel=\"previous\"");

        var result = HeaderParser.ParseLinkHeader(headers);

        Assert.That(result.PreviousPage, Is.EqualTo("prevPageCursor456"));
        Assert.That(result.NextPage, Is.Null);
    }

    [Test]
    public void ParseLinkHeader_WithMultipleLinksInSingleString_ReturnsCorrectPageInfo()
    {
        var headers = new HttpResponseMessage().Headers;
        headers.Add("Link", "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=nextPageCursor123>; rel=\"next\", " + "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=prevPageCursor456>; rel=\"previous\"");

        var result = HeaderParser.ParseLinkHeader(headers);

        Assert.That(result.NextPage, Is.EqualTo("nextPageCursor123"));
        Assert.That(result.PreviousPage, Is.EqualTo("prevPageCursor456"));
    }

    [Test]
    public void ParseLinkHeader_WithMultipleLinkHeaders_ReturnsCorrectPageInfo()
    {
        var headers = new HttpResponseMessage().Headers;
        headers.Add("Link", "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=nextPageCursor123>; rel=\"next\"");
        headers.Add("Link", "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=prevPageCursor456>; rel=\"previous\"");

        var result = HeaderParser.ParseLinkHeader(headers);

        Assert.That(result.NextPage, Is.EqualTo("nextPageCursor123"));
        Assert.That(result.PreviousPage, Is.EqualTo("prevPageCursor456"));
    }

    [Test]
    public void ParseLinkHeader_WithNoLinkHeader_ReturnsEmptyPageInfo()
    {
        var headers = new HttpResponseMessage().Headers;

        var result = HeaderParser.ParseLinkHeader(headers);

        Assert.That(result.NextPage, Is.Null);
        Assert.That(result.PreviousPage, Is.Null);
    }

    [Test]
    public void Handle_WhenThrowErrorIsTrue_ThrowsException()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error";

        var thrownEx = Assert.Throws<Exception>(() =>
            ErrorHandler.Handle(ex, true, customMessage));

        Assert.That(thrownEx.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(thrownEx.InnerException, Is.EqualTo(ex));
    }

    [Test]
    public void Handle_WhenThrowErrorIsFalse_ReturnsErrorResult()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error";

        var result = ErrorHandler.Handle(ex, false, customMessage);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Orders, Is.Null);
        Assert.That(result.PageInfo, Is.Null);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(result.Error.AdditionalInfo, Is.EqualTo(ex));
    }
}