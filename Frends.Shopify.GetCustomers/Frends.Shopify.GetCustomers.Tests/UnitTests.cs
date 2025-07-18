using System;
using System.Threading;
using System.Threading.Tasks;
using dotenv.net;
using Frends.Shopify.GetCustomers.Definitions;
using Frends.Shopify.GetCustomers.Helpers;
using NUnit.Framework;

namespace Frends.Shopify.GetCustomers.Tests;

/// <summary>
/// Test cases for Shopify GetCustomers task.
/// </summary>
[TestFixture]
public class UnitTests
{
    private readonly string shopName = "frendstemplates";
    private readonly string accessToken;
    private readonly string apiVersion = "2025-07";
    private readonly string customerId = "7894974431335";
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
            CustomerId = customerId,
        };

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [Test]
    public async Task GetCustomers_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        var result = await Shopify.GetCustomer(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Customer, Is.Not.Null);
        Assert.That(result.Customer["id"]?.ToString(), Is.EqualTo(customerId));
    }

    [Test]
    public async Task GetCustomers_WithFields_SuccessTest()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Assert.Ignore("AccessToken not configured in environment variables. Test skipped.");
            return;
        }

        options.Fields = ["id", "email", "first_name"];

        var result = await Shopify.GetCustomer(input, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Customer, Is.Not.Null);
        Assert.That(result.Customer["id"]?.ToString(), Is.EqualTo(customerId));
        Assert.That(result.Customer["email"], Is.Not.Null);
        Assert.That(result.Customer["first_name"], Is.Not.Null);
    }

    [Test]
    public void GetCustomers_ShopNameValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = null,
            AccessToken = accessToken,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetCustomer(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ShopName is required"));
    }

    [Test]
    public void GetCustomers_AccessTokenValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = null,
            ApiVersion = apiVersion,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetCustomer(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("AccessToken is required"));
    }

    [Test]
    public void GetCustomers_ApiVersionValidationFailureTest()
    {
        var invalidConnection = new Connection
        {
            ShopName = shopName,
            AccessToken = accessToken,
            ApiVersion = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetCustomer(input, invalidConnection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("ApiVersion is required"));
    }

    [Test]
    public void GetCustomers_CustomerIdValidationFailureTest()
    {
        var invalidInput = new Input
        {
            CustomerId = null,
        };

        var ex = Assert.ThrowsAsync<Exception>(() =>
            Shopify.GetCustomer(invalidInput, connection, options, CancellationToken.None));

        Assert.That(ex.Message, Does.Contain("CustomerId is required"));
    }

    [Test]
    public async Task GetCustomers_ErrorHandlingTest()
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
            CustomerId = "999999999999999999",
        };

        var result = await Shopify.GetCustomer(invalidInput, connection, options, CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Does.Contain("Custom error message"));
    }

    [Test]
    public void GetCustomers_Handle_WhenThrowErrorIsTrue_ThrowsException()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error context";

        var thrownEx = Assert.Throws<Exception>(() =>
            ErrorHandler.Handle(ex, true, customMessage));

        Assert.That(thrownEx.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(thrownEx.InnerException, Is.EqualTo(ex));
    }

    [Test]
    public void GetCustomers_Handle_WhenThrowErrorIsFalse_ReturnsErrorResult()
    {
        var ex = new Exception("Test exception");
        const string customMessage = "Custom error context";

        var result = ErrorHandler.Handle(ex, false, customMessage);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Customer, Is.Null);
        Assert.That(result.Error, Is.Not.Null);
        Assert.That(result.Error.Message, Is.EqualTo($"{customMessage} {ex.Message}"));
        Assert.That(result.Error.AdditionalInfo, Is.EqualTo(ex));
    }
}