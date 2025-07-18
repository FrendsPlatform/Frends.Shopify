using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Shopify.GetCustomers.Definitions;

/// <summary>
/// Input parameters for Shopify GetCustomer task.
/// </summary>
public class Input
{
    /// <summary>
    /// The ID of the customer to retrieve.
    /// </summary>
    /// <example>1234567890123</example>
    [Display(Name = "Customer Id")]
    public string CustomerId { get; set; }
}