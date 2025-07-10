using System;
using Frends.Shopify.CreateProduct.Definitions;

namespace Frends.Shopify.CreateProduct.Helpers
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
        /// Object { bool Success, JObject CreatedProduct, Error Error { string Message, Exception AdditionalInfo } }
        /// </returns>
        public static Result Handle(Exception ex, bool throwError, string customMessage)
        {
            var error = new Error
            {
                Message = $"{customMessage} {ex.Message}",
                AdditionalInfo = ex,
            };

            if (throwError)
                throw new Exception(error.Message, ex);

            return new Result(false, null, error);
        }
    }
}