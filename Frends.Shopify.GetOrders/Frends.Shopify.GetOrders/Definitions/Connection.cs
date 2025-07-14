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
    [DefaultValue("")]
    public string ShopDomain { get; set; }

    /// <summary>
    /// Shopify Admin API access token for authenticating requests (from private app or OAuth).
    /// </summary>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    [PasswordPropertyText]
    public string AccessToken { get; set; }
}
