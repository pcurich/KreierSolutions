using Ks.Core.Domain.Localization;

namespace Ks.Core.Domain.System
{
    /// <summary>
    /// Represents a Ks System
    /// </summary>
    public partial class KsSystem : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the store URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled
        /// </summary>
        public bool SslEnabled { get; set; }

        /// <summary>
        /// Gets or sets the store secure URL (HTTPS)
        /// </summary>
        public string SecureUrl { get; set; }

        /// <summary>
        /// Gets or sets the comma separated list of possible HTTP_HOST values
        /// </summary>
        public string Hosts { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the default language for this system; 0 is set when we use the default language display order
        /// </summary>
        public int DefaultLanguageId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company address
        /// </summary>
        public string CompanyAddress { get; set; }

        /// <summary>
        /// Gets or sets the store phone number
        /// </summary>
        public string CompanyPhoneNumber { get; set; }
    }
}