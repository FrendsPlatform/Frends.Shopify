using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetCustomer.Definitions;

/// <summary>
/// Connection parameters for Shopify GetCustomer task.
/// </summary>
public class Connection
{
    /// <summary>
    /// The subdomain of the Shopify store (e.g., myshop for myshop.myshopify.com).
    /// </summary>
    /// <example>myshop</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ShopName { get; set; }

    /// <summary>
    /// Shopify Admin API access token for authenticating requests.
    /// </summary>
    /// <example>
    /// Private:
    /// <code>shpat_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx</code>
    /// OAuth:
    /// <code>shpca_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx</code>
    /// </example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AccessToken { get; set; }

    /// <summary>
    /// The version of Shopify API to use
    /// </summary>
    /// <example>2025-07</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ApiVersion { get; set; }
}