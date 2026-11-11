namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Pagination cursors.
/// </summary>
public class PageInfo
{
    /// <summary>
    /// Gets or sets the cursor for the next page.
    /// </summary>
    /// <example><code>eyJsYXN0X2lkIjo0MDkzNTg3ODQsImxhc3RfdmFsdWUiOiI0MDkzNTg3ODQifQ==</code></example>
    public string NextPage { get; set; }

    /// <summary>
    /// Gets or sets the cursor for the previous page.
    /// </summary>
    /// <example><code>eyJsYXN0X2lkIjo0MDkzNTg3ODQsImxhc3RfdmFsdWUiOiI0MDkzNTg3ODQifQ==</code></example>
    public string PreviousPage { get; set; }
}
