using Ks.Core.Configuration;

namespace Ks.Core.Domain
{
    public class KsSystemInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether "powered by nopCommerce" text should be displayed.
        /// Please find more info at http://www.nopcommerce.com/copyrightremoval.aspx
        /// </summary>
        public bool HidePoweredByKreierSolutions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether store is closed
        /// </summary>
        public bool StoreClosed { get; set; }

        /// <summary>
        /// Gets or sets a default store theme
        /// </summary>
        public string DefaultStoreTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select a theme
        /// </summary>
        public bool AllowCustomerToSelectTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in web site (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInWebSite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        public bool DisplayEuCookieLawWarning { get; set; }

        /// <summary>
        /// Gets or sets a value of Facebook page URL of the site
        /// </summary>
        public string FacebookLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Twitter page URL of the site
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        /// Gets or sets a value of YouTube channel URL of the site
        /// </summary>
        public string YoutubeLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Google+ page URL of the site
        /// </summary>
        public string GooglePlusLink { get; set; }
    }
}