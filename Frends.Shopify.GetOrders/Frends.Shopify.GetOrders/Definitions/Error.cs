using System;

namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets or sets a summary of the error.
    /// </summary>
    /// <example><code>Shopify API error: 401 - Invalid access token.</code></example>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets additional information about the error.
    /// </summary>
    /// <example><code>new Exception("Invalid access token")</code></example>
    public Exception AdditionalInfo { get; set; }
}
