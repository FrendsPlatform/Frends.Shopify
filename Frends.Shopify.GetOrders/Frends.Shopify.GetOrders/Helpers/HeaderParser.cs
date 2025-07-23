using System;
using System.Net.Http.Headers;
using Frends.Shopify.GetOrders.Definitions;

namespace Frends.Shopify.GetOrders.Helpers
{
    /// <summary>
    /// HTTP header parsing
    /// </summary>
    public static class HeaderParser
    {
        /// <summary>
        /// Parses Shopify's Link header to extract pagination cursors.
        /// </summary>
        /// <param name="headers">HTTP response headers</param>
        /// <returns>PageInfo containing NextPage and PreviousPage cursors</returns>
        public static PageInfo ParseLinkHeader(HttpResponseHeaders headers)
        {
            var pageInfo = new PageInfo();

            if (!headers.TryGetValues("Link", out var linkHeaderValues))
                return pageInfo;

            foreach (var link in linkHeaderValues)
            {
                var links = link.Split(',');
                foreach (var item in links)
                {
                    if (!item.Contains("page_info=")) continue;

                    if (item.Contains("rel=\"next\""))
                    {
                        var cursor = ExtractPageInfoCursor(item);
                        if (!string.IsNullOrEmpty(cursor))
                            pageInfo.NextPage = cursor;
                    }
                    else if (item.Contains("rel=\"previous\""))
                    {
                        var cursor = ExtractPageInfoCursor(item);
                        if (!string.IsNullOrEmpty(cursor))
                            pageInfo.PreviousPage = cursor;
                    }
                }
            }

            return pageInfo;
        }

        /// <summary>
        /// Extracts the page_info cursor value from a link header item
        /// </summary>
        private static string ExtractPageInfoCursor(string linkItem)
        {
            var pageInfoIndex = linkItem.IndexOf("page_info=", StringComparison.Ordinal);
            if (pageInfoIndex == -1) return string.Empty;

            var start = pageInfoIndex + "page_info=".Length;
            if (start >= linkItem.Length) return string.Empty;

            var end = linkItem.IndexOfAny(['>', '&'], start);
            if (end == -1) end = linkItem.Length;

            if (end <= start) return string.Empty;

            var cursor = linkItem[start..end];
            return Uri.UnescapeDataString(cursor);
        }
    }
}