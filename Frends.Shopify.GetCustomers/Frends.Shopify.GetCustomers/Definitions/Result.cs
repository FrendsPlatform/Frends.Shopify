using Newtonsoft.Json.Linq;

namespace Frends.Shopify.GetCustomers.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// Result of retrieving a Shopify customer.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="customer">The retrieved customer data.</param>
    /// <param name="error">Error details if the operation failed.</param>
    internal Result(bool success, JObject customer, Error error = null)
    {
        Success = success;
        Customer = customer;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the retrieval was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The customer object returned by Shopify.
    /// </summary>
    /// <example>
    /// {
    ///   "id": 1234567890123,
    ///   "email": "test@email.com",
    ///   "created_at": "2025-07-17T15:27:48+03:00",
    ///   "updated_at": "2025-07-17T15:27:48+03:00",
    ///   "first_name": "Test",
    ///   "last_name": "User",
    ///   "orders_count": 0,
    ///   "state": "disabled",
    ///   "total_spent": "0.00",
    ///   "last_order_id": null,
    ///   "note": "",
    ///   "verified_email": true,
    ///   "multipass_identifier": null,
    ///   "tax_exempt": false,
    ///   "tags": "",
    ///   "last_order_name": null,
    ///   "currency": "EUR",
    ///   "phone": null,
    ///   "addresses": [
    ///     {
    ///       "id": 1234567890123,
    ///       "customer_id": 1234567890123,
    ///       "first_name": "Test",
    ///       "last_name": "User",
    ///       "company": "",
    ///       "address1": "",
    ///       "address2": "",
    ///       "city": "",
    ///       "province": "",
    ///       "country": "Finland",
    ///       "zip": "",
    ///       "phone": "",
    ///       "name": "Test User",
    ///       "province_code": null,
    ///       "country_code": "FI",
    ///       "country_name": "Finland",
    ///       "default": true
    ///     }
    ///   ],
    /// }
    /// </example>
    public JObject Customer { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}