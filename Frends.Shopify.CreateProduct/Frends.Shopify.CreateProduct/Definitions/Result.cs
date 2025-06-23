﻿namespace Frends.Shopify.CreateProduct.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// The created product object returned by Shopify.
    /// </summary>
    public object CreatedProduct { get; private set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; private set; }

    internal Result(bool success, object createdProduct, Error error = null)
    {
        Success = success;
        CreatedProduct = createdProduct;
        Error = error;
    }
}
