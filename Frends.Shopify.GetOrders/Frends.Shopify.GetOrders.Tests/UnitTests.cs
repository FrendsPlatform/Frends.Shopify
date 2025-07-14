using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetOrders.Definitions;
using Frends.Shopify.GetOrders.Helpers;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Shopify.GetOrders.Tests;

/// <summary>
/// Test cases for Shopify GetOrders task.
/// </summary>
[TestFixture]
public class UnitTests
{
    private Mock<IShopifyApiClient> mockShopifyClient;
    private Connection connection;
    private Input input;
    private Options options;

    [SetUp]
    public void Setup()
    {
        mockShopifyClient = new Mock<IShopifyApiClient>();

        connection = new Connection
        {
            ShopDomain = "test-shop.myshopify.com",
            AccessToken = "test-token",
        };

        input = new Input
        {
            CreatedAtMin = "2024-01-01T00:00:00Z",
            CreatedAtMax = "2024-01-31T23:59:59Z",
            Status = "any",
            FulfillmentStatus = "shipped",
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
            Limit = 50,
            Fields = "id,created_at,line_items",
        };
    }

    [Test]
    public void GetOrders_ShopDomainValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopDomain = null,
            AccessToken = "test-token",
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() => Shopify.GetOrders(input, invalidConnection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ShopDomain is required"));
        mockShopifyClient.Verify(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void GetOrders_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopDomain = "test-shop.myshopify.com",
            AccessToken = null,
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(() => Shopify.GetOrders(input, invalidConnection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
        mockShopifyClient.Verify(
            x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetOrders_SuccessTest()
    {
        var mockOrders = JArray.FromObject(new[]
        {
            new { id = "1001", created_at = "2024-01-15T10:00:00Z" },
            new { id = "1002", created_at = "2024-01-20T11:00:00Z" },
        });

        var mockResponse = new OrdersResponse
        {
            Orders = mockOrders,
            PageInfo = new PageInfo { NextPage = "next-page-cursor" },
        };

        mockShopifyClient.Setup(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Orders, Is.Not.Null);
        Assert.That(result.Orders.Count, Is.EqualTo(2));
        Assert.That(result.PageInfo.NextPage, Is.EqualTo("next-page-cursor"));
    }

    [Test]
    public async Task GetOrders_WithPageInfo_SuccessTest()
    {
        options.PageInfo = "next-page-cursor";
        var mockOrders = JArray.FromObject(new[]
        {
            new { id = "1003", created_at = "2024-01-25T12:00:00Z" },
        });

        var mockResponse = new OrdersResponse
        {
            Orders = mockOrders,
            PageInfo = new PageInfo { PreviousPage = "prev-page-cursor" },
        };

        mockShopifyClient.Setup(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), options.PageInfo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Orders.Count, Is.EqualTo(1));
        Assert.That(result.PageInfo.PreviousPage, Is.EqualTo("prev-page-cursor"));
    }

    [Test]
    public async Task GetOrders_WithStatusFilter_SuccessTest()
    {
        input.Status = "closed";
        var mockOrders = JArray.FromObject(new[]
        {
            new { id = "1004", created_at = "2024-01-10T09:00:00Z", status = "closed" },
        });

        mockShopifyClient.Setup(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<string>(), input.Status, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrdersResponse { Orders = mockOrders });

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Orders.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetOrders_WithFieldsFilter_SuccessTest()
    {
        options.Fields = "id,created_at,total_price";
        var mockOrders = JArray.FromObject(new[]
        {
            new { id = "1005", created_at = "2024-01-05T08:00:00Z", total_price = "10.99" },
        });

        mockShopifyClient.Setup(x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), options.Fields, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrdersResponse { Orders = mockOrders });

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Orders.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetOrders_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        mockShopifyClient.Setup(
            x => x.GetOrdersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error occurred"));

        var result = await Shopify.GetOrders(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.EqualTo("Custom error message"));
        Assert.That(result.Error.AdditionalInfo.Message, Is.EqualTo("API error occurred"));
    }

    [Test]
    public void Handle_WhenThrowErrorIsTrue_ThrowsException()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error";

        var thrownEx = Assert.Throws<Exception>(() => ErrorHandler.Handle(ex, true, customMessage));

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

    [Test]
    public void ShopifyApiClient_GetOrdersAsync_AfterDispose_ThrowsException()
    {
        var client = new ShopifyApiClient(connection);
        client.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(() => client.GetOrdersAsync(input.CreatedAtMin, input.CreatedAtMax, input.Status, input.FulfillmentStatus, options.Fields, options.Limit, options.PageInfo, CancellationToken.None));
    }

    [Test]
    public void ParseLinkHeader_WithNextPage_ReturnsCorrectPageInfo()
    {
        var linkHeader = new[]
        {
            "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=nextPageCursor123>; rel=\"next\"",
        };

        var result = ShopifyApiClient.ParseLinkHeader(linkHeader);

        Assert.That(result.NextPage, Is.EqualTo("nextPageCursor123"));
        Assert.That(result.PreviousPage, Is.Null);
    }

    [Test]
    public void ParseLinkHeader_WithPreviousPage_ReturnsCorrectPageInfo()
    {
        var linkHeader = new[]
        {
            "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=prevPageCursor456>; rel=\"previous\"",
        };

        var result = ShopifyApiClient.ParseLinkHeader(linkHeader);

        Assert.That(result.PreviousPage, Is.EqualTo("prevPageCursor456"));
        Assert.That(result.NextPage, Is.Null);
    }

    [Test]
    public void ParseLinkHeader_WithMultipleLinksInSingleString_ReturnsCorrectPageInfo()
    {
        var linkHeader = new[]
        {
            "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=nextPageCursor123>; rel=\"next\", " +
            "<https://test-shop.myshopify.com/admin/api/2023-10/orders.json?page_info=prevPageCursor456>; rel=\"previous\"",
        };

        var result = ShopifyApiClient.ParseLinkHeader(linkHeader);

        Assert.That(result.NextPage, Is.EqualTo("nextPageCursor123"));
        Assert.That(result.PreviousPage, Is.EqualTo("prevPageCursor456"));
    }
}