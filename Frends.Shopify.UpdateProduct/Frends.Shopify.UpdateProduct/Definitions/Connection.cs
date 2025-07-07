using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.UpdateProduct.Definitions;

/// <summary>
/// Connection parameters for Shopify UpdateProduct task.
/// </summary>
public class Connection
{
    /// <summary>
    /// The subdomain of the Shopify store (e.g., myshop for myshop.myshopify.com).
    /// </summary>
    /// <example>myshop</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    [PasswordPropertyText]
    public string ShopName { get; set; }

    /// <summary>
    /// Shopify Admin API access token for authenticating requests.
    /// </summary>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    [PasswordPropertyText]
    public string AccessToken { get; set; }

    /// <summary>
    /// The version of Shopify API to use
    /// </summary>
    /// <example>2023-10</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ApiVersion { get; set; }
}