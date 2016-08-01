using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Localization;


namespace Ks.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        Customer CurrentCustomer { get; set; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }
        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        Currency WorkingCurrency { get; set; }
  
        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
