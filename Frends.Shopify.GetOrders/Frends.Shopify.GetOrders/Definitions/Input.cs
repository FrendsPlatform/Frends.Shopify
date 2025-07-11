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
    [Display(Name = "Created At Min")]
    public string CreatedAtMin { get; set; }

    /// <summary>
    /// Optional. End date/time in ISO 8601 format.
    /// </summary>
    [Display(Name = "Created At Max")]
    public string CreatedAtMax { get; set; }

    /// <summary>
    /// Optional. Filter orders by status, e.g. open, closed, cancelled, or any. Default is any.
    /// </summary>
    [Display(Name = "Status")]
    [DefaultValue("any")]
    public string Status { get; set; }

    /// <summary>
    /// Optional. Filter by fulfillment status, e.g. shipped, partial, unshipped.
    /// </summary>
    [Display(Name = "FulfillmentStatus")]
    public string FulfillmentStatus { get; set; }
}
