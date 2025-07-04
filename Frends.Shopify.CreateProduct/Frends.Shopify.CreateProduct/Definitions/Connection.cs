using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.CreateProduct.Definitions;

/// <summary>
/// Connection parameters for Shopify CreateProduct task.
/// </summary>
public class Connection
{
    /// <summary>
    /// The Shopify store subdomain (e.g., myshop for myshop.myshopify.com).
    /// </summary>
    /// <example>myshop</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    [PasswordPropertyText]
    public string ShopName { get; set; }

    /// <summary>
    /// Shopify Admin API access token (private or custom app token).
    /// </summary>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    [PasswordPropertyText]
    public string AccessToken { get; set; }

    /// <summary>
    /// Shopify Admin API version to use.
    /// </summary>
    /// <example>2023-10</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ApiVersion { get; set; }
}
