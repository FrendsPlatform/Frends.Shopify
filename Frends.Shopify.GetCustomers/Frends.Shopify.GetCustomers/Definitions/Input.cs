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
    /// <example>
    /// <code>2025-01-01T00:00:00</code>
    /// </example>
    [Display(Name = "Created At Min")]
    public string CreatedAtMin { get; set; }

    /// <summary>
    /// Optional. End date/time in ISO 8601 format.
    /// </summary>
    /// <example>
    /// <code>2025-12-31T23:59:59</code>
    /// </example>
    [Display(Name = "Created At Max")]
    public string CreatedAtMax { get; set; }

    /// <summary>
    /// Optional. Filter by customer state. Default is any.
    /// </summary>
    /// <example>
    /// <code>ENABLED</code>
    /// <code>DISABLED</code>
    /// <code>INVITED</code>
    /// <code>DECLINED</code>
    /// <code>ANY</code>
    /// </example>
    [Display(Name = "State")]
    [DefaultValue("any")]
    public string State { get; set; }
}