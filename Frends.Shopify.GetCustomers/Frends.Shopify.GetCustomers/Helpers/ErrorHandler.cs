using System;
using Frends.Shopify.GetCustomers.Definitions;

namespace Frends.Shopify.GetCustomers.Helpers
{
    /// <summary>
    /// Error handling
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Handles exceptions and creates an error response or throws an exception.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="throwError">If true, throws the exception. If false, returns a failure result with error details.</param>
        /// <param name="customMessage">Additional custom message included in the error.</param>
        /// <returns>
        /// Object { bool Success, List customers, PageInfo pageInfo { string NextPage, string PreviousPage }, Error Error { string Message, Exception AdditionalInfo } }
        /// </returns>
        public static Result Handle(Exception ex, bool throwError, string customMessage)
        {
            var error = new Error
            {
                Message = string.IsNullOrWhiteSpace(customMessage)
                    ? ex.Message
                    : $"{customMessage.Trim()} {ex.Message}".Trim(),
                AdditionalInfo = ex,
            };

            if (throwError)
                throw new Exception(error.Message, ex);

            return new Result(false, null, error);
        }
    }
}