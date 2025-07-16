using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.Shopify.GetCustomers.Definitions;
using Frends.Shopify.GetCustomers.Helpers;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Shopify.GetCustomers.Tests;

/// <summary>
/// Test cases for Shopify GetCustomers task.
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
            State = "any",
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
            Limit = 50,
            Fields = "id,created_at,line_items",
        };
    }

    [Test]
    public void GetCustomers_ShopDomainValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopDomain = null,
            AccessToken = "test-token",
        };

        var ex = Assert.ThrowsAsync<Exception>(() => Shopify.GetCustomers(input, invalidConnection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("ShopDomain is required"));
        mockShopifyClient.Verify(x => x.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void GetCustomers_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopDomain = "test-shop.myshopify.com",
            AccessToken = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() => Shopify.GetCustomers(input, invalidConnection, options, CancellationToken.None, mockShopifyClient.Object));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
        mockShopifyClient.Verify(
            x => x.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetCustomers_SuccessTest()
    {
        var mockCustomers = JArray.FromObject(new[]
        {
            new { id = "1001", created_at = "2024-01-15T10:00:00Z" },
            new { id = "1002", created_at = "2024-01-20T11:00:00Z" },
        });

        var mockResponse = new CustomersResponse
        {
            Customers = mockCustomers,
            PageInfo = new PageInfo { NextPage = "next-page-cursor" },
        };

        mockShopifyClient.Setup(x => x.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var result = await Shopify.GetCustomers(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Customers, Is.Not.Null);
        Assert.That(result.Customers.Count, Is.EqualTo(2));
        Assert.That(result.PageInfo.NextPage, Is.EqualTo("next-page-cursor"));
    }

    [Test]
    public async Task GetCustomers_WithPageInfo_SuccessTest()
    {
        options.PageInfo = "next-page-cursor";
        var mockCustomers = JArray.FromObject(new[]
        {
            new { id = "1003", created_at = "2024-01-25T12:00:00Z" },
        });

        var mockResponse = new CustomersResponse
        {
            Customers = mockCustomers,
            PageInfo = new PageInfo { PreviousPage = "prev-page-cursor" },
        };

        mockShopifyClient.Setup(x => x.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), options.PageInfo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        var result = await Shopify.GetCustomers(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Customers.Count, Is.EqualTo(1));
        Assert.That(result.PageInfo.PreviousPage, Is.EqualTo("prev-page-cursor"));
    }

    [Test]
    public async Task GetCustomers_WithStateFilter_SuccessTest()
    {
        input.State = "ENABLED";
        var mockCustomers = JArray.FromObject(new[]
        {
        new { id = "2001", state = "enabled" },
        });

        mockShopifyClient.Setup(x => x.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), input.State, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomersResponse { Customers = mockCustomers });

        var result = await Shopify.GetCustomers(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Customers.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetCustomers_WithFieldsFilter_SuccessTest()
    {
        options.Fields = "id,created_at,total_price";
        var mockCustomers = JArray.FromObject(new[]
        {
            new { id = "1005", created_at = "2024-01-05T08:00:00Z", total_price = "10.99" },
        });

        mockShopifyClient.Setup(x => x.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), options.Fields, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomersResponse { Customers = mockCustomers });

        var result = await Shopify.GetCustomers(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Customers.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetCustomers_ErrorHandlingTest()
    {
        options.ThrowErrorOnFailure = false;
        options.ErrorMessageOnFailure = "Custom error message";

        mockShopifyClient.Setup(
            x => x.GetCustomersAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error occurred"));

        var result = await Shopify.GetCustomers(input, connection, options, CancellationToken.None, mockShopifyClient.Object);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
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
        Assert.That(result.Customers, Is.Null);
        Assert.That(result.PageInfo, Is.Null);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(result.Error.AdditionalInfo, Is.EqualTo(ex));
    }

    [Test]
    public void ShopifyApiClient_GetCustomersAsync_AfterDispose_ThrowsException()
    {
        var client = new ShopifyApiClient(connection);
        client.Dispose();

        Assert.ThrowsAsync<ObjectDisposedException>(() => client.GetCustomersAsync(input.CreatedAtMin, input.CreatedAtMax, input.State, options.Fields, options.Limit, options.PageInfo, CancellationToken.None));
    }

    [Test]
    public void ParseLinkHeader_WithNextPage_ReturnsCorrectPageInfo()
    {
        var linkHeader = new[]
        {
            "<https://test-shop.myshopify.com/admin/api/2023-10/customers.json?page_info=nextPageCursor123>; rel=\"next\"",
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
            "<https://test-shop.myshopify.com/admin/api/2023-10/customers.json?page_info=prevPageCursor456>; rel=\"previous\"",
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
            "<https://test-shop.myshopify.com/admin/api/2023-10/customers.json?page_info=nextPageCursor123>; rel=\"next\", " +
            "<https://test-shop.myshopify.com/admin/api/2023-10/customers.json?page_info=prevPageCursor456>; rel=\"previous\"",
        };

        var result = ShopifyApiClient.ParseLinkHeader(linkHeader);

        Assert.That(result.NextPage, Is.EqualTo("nextPageCursor123"));
        Assert.That(result.PreviousPage, Is.EqualTo("prevPageCursor456"));
    }
}