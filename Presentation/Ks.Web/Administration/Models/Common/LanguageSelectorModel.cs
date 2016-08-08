using System.Collections.Generic;
using Ks.Admin.Models.Localization;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Common
{
    public partial class LanguageSelectorModel : BaseKsModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }
    }
}