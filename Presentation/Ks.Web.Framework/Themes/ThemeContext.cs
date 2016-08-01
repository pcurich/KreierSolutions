using System;
using System.Linq;
using Ks.Core;
using Ks.Core.Domain;
using Ks.Core.Domain.Customers;
using Ks.Services.Common;

namespace Ks.Web.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IWorkContext _workContext;
        private readonly IKsSystemContext _ksSystemContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly KsSystemInformationSettings _ksSystemInformationSettings;
        private readonly IThemeProvider _themeProvider;

        private bool _themeIsCached;
        private string _cachedThemeName;

        public ThemeContext(IWorkContext workContext,
            IKsSystemContext ksSystemContext,
            IGenericAttributeService genericAttributeService,
            KsSystemInformationSettings ksSystemInformationSettings, 
            IThemeProvider themeProvider)
        {
            this._workContext = workContext;
            this._ksSystemContext = ksSystemContext;
            this._genericAttributeService = genericAttributeService;
            this._ksSystemInformationSettings = ksSystemInformationSettings;
            this._themeProvider = themeProvider;
        }

        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        public string WorkingThemeName
        {
            get
            {
                if (_themeIsCached)
                    return _cachedThemeName;

                string theme = "";
                if (_ksSystemInformationSettings.AllowCustomerToSelectTheme)
                {
                    if (_workContext.CurrentCustomer != null)
                        theme = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.WorkingThemeName, _genericAttributeService, _ksSystemContext.CurrentSystem.Id);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _ksSystemInformationSettings.DefaultStoreTheme;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                {
                    var themeInstance = _themeProvider.GetThemeConfigurations()
                        .FirstOrDefault();
                    if (themeInstance == null)
                        throw new Exception("No theme could be loaded");
                    theme = themeInstance.ThemeName;
                }
                
                //cache theme
                this._cachedThemeName = theme;
                this._themeIsCached = true;
                return theme;
            }
            set
            {
                if (!_ksSystemInformationSettings.AllowCustomerToSelectTheme)
                    return;

                if (_workContext.CurrentCustomer == null)
                    return;

                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.WorkingThemeName, value, _ksSystemContext.CurrentSystem.Id);

                //clear cache
                this._themeIsCached = false;
            }
        }
    }
}
