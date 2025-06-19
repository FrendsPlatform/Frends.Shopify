using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.CreateProduct.Definitions;

/// <summary>
/// Connection parameters for Shopify CreateProduct task.
/// </summary>
public class Connection
{
    /// <summary>
    /// The Shopify store subdomain.
    /// </summary>
    /// <example>myshop for myshop.myshopify.com</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ShopName { get; set; }

    /// <summary>
    /// Shopify Admin API access token (private or custom app token).
    /// </summary>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string AccessToken { get; set; }

    /// <summary>
    /// Shopify Admin API version to use.
    /// </summary>
    /// <example>2023-10</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ApiVersion { get; set; }
}
