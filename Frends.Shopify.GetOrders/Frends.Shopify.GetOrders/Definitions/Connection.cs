using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Gets or sets the Shopify store domain.
    /// </summary>
    /// <example><code>your-store.myshopify.com</code></example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ShopDomain { get; set; }

    /// <summary>
    /// Gets or sets the Shopify Admin API access token for authenticating requests (from a private app or OAuth).
    /// </summary>
    /// <example>
    /// Private:
    /// <code>example-private-app-token-from-shopify-admin</code>
    /// OAuth:
    /// <code>example-oauth-access-token-from-shopify</code>
    /// </example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the Shopify API version to use.
    /// </summary>
    /// <example><code>2025-07</code></example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ApiVersion { get; set; }
}
