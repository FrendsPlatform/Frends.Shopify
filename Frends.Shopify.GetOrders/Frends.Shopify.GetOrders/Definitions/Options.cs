using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Gets or sets an optional comma-separated list of fields to return.
    /// </summary>
    /// <example>
    /// <code>id,created_at,total_price</code>
    /// </example>
    public string Fields { get; set; }

    /// <summary>
    /// Gets or sets an optional pagination cursor, such as a next-page or previous-page cursor.
    /// </summary>
    /// <example>
    /// <code>eyJsYXN0X2lkIjo0MDkzNTg3ODQsImxhc3RfdmFsdWUiOiI0MDkzNTg3ODQifQ==</code>
    /// </example>
    public string PageInfo { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results per page (1-250). The default is 50.
    /// </summary>
    /// <example><code>50</code></example>
    [DefaultValue(50)]
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to throw an error when the task fails. The default is true.
    /// </summary>
    /// <example><code>false</code></example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Gets or sets a custom error message to use when the task fails.
    /// </summary>
    /// <example><code>Failed to retrieve orders from the store</code></example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ErrorMessageOnFailure { get; set; }
}
