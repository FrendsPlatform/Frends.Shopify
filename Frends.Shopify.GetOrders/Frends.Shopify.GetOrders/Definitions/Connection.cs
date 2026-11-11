using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Your Shopify store domain.
    /// </summary>
    /// <example>your-store.myshopify.com</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ShopDomain { get; set; }

    /// <summary>
    /// Shopify Admin API access token for authenticating requests (from private app or OAuth).
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
