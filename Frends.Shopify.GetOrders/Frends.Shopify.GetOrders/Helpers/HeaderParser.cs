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
                        var start = item.IndexOf("page_info=", StringComparison.Ordinal) + "page_info=".Length;
                        var end = item.IndexOfAny(['>', '&'], start);
                        if (start > 0 && end > start)
                            pageInfo.NextPage = item[start..end];
                    }
                    else if (item.Contains("rel=\"previous\""))
                    {
                        var start = item.IndexOf("page_info=", StringComparison.Ordinal) + "page_info=".Length;
                        var end = item.IndexOfAny(['>', '&'], start);
                        if (start > 0 && end > start)
                            pageInfo.PreviousPage = item[start..end];
                    }
                }
            }

            return pageInfo;
        }
    }
}