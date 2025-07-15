using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetCustomers.Definitions;

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
    /// Optional. Filter by customer state: ENABLED, DISABLED, INVITED, DECLINED, or any. Default is any.
    /// </summary>
    [Display(Name = "State")]
    [DefaultValue("any")]
    public string State { get; set; }
}