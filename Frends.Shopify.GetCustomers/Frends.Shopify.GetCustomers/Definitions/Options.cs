using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetCustomers.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Optional. Comma-separated list of fields to return.
    /// </summary>
    /// <example>
    /// <code>id,first_name,last_name,email,created_at,total_spent,orders_count</code>
    /// </example>
    public string Fields { get; set; }

    /// <summary>
    /// Optional. Pagination cursor, e.g. nextPage or previousPage.
    /// </summary>
    /// <example>
    /// <code>eyJsYXN0X2lkIjo0MDkzNTg3ODQsImxhc3RfdmFsdWUiOiI0MDkzNTg3ODQifQ==</code>
    /// </example>
    public string PageInfo { get; set; }

    /// <summary>
    /// Optional. Max number of results per page (1-250). Default is 50.
    /// </summary>
    /// <example>50</example>
    [DefaultValue(50)]
    public int Limit { get; set; }

    /// <summary>
    /// Whether to throw an error on failure. True by default.
    /// </summary>
    /// <example>False</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Failed to retrieve customers.</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}