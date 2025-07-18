using System;

namespace Frends.Shopify.GetCustomers.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to retrieve customers.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception Exception }</example>
    public Exception AdditionalInfo { get; set; }
}