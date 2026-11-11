namespace Frends.Shopify.GetOrders.Definitions;

/// <summary>
/// Pagination cursor.
/// </summary>
public class PageInfo
{
    /// <summary>
    /// Pagination cursor for nextPage.
    /// </summary>
    /// <example>eyJsYXN0X2lkIjo0MDkzNTg3ODQsImxhc3RfdmFsdWUiOiI0MDkzNTg3ODQifQ==</example>
    public string NextPage { get; set; }

    /// <summary>
    /// Pagination cursor for previousPage.
    /// </summary>
    /// <example>eyJsYXN0X2lkIjo0MDkzNTg3ODQsImxhc3RfdmFsdWUiOiI0MDkzNTg3ODQifQ==</example>
    public string PreviousPage { get; set; }
}
