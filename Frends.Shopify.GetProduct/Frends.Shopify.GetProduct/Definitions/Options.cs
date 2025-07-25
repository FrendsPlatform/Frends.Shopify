﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetProduct.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Optional. List of specific fields to include in the response.
    /// </summary>
    /// <example>
    /// <code>id,title,handle,status,variants,images</code>
    /// </example>
    public string[] Fields { get; set; }

    /// <summary>
    /// Whether to throw an error on failure. True by default.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Failed to retrieve product.</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ErrorMessageOnFailure { get; set; }
}
