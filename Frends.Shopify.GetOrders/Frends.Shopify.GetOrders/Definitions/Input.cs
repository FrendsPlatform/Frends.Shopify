using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Optional. Start date/time in ISO 8601 format.
    /// </summary>
    /// <example>
    /// <code>"2025-01-01T00:00:00Z"</code>
    /// </example>
    [Display(Name = "Created At Min")]
    public string CreatedAtMin { get; set; }

    /// <summary>
    /// Optional. End date/time in ISO 8601 format.
    /// </summary>
    /// <example>
    /// <code>"2025-12-31T23:59:59Z"</code>
    /// </example>
    [Display(Name = "Created At Max")]
    public string CreatedAtMax { get; set; }

    /// <summary>
    /// Optional. Filter orders by status. Default is any.
    /// </summary>
    /// <example>
    /// <code>open, closed, cancelled, any</code>
    /// </example>
    [Display(Name = "Status")]
    [DefaultValue("any")]
    [DisplayFormat(DataFormatString = "Text")]
    public string Status { get; set; }

    /// <summary>
    /// Optional. Filter by fulfillment status.
    /// </summary>
    /// <example>
    /// <code>shipped, partial, unshipped</code>
    /// </example>
    [Display(Name = "FulfillmentStatus")]
    public string FulfillmentStatus { get; set; }
}