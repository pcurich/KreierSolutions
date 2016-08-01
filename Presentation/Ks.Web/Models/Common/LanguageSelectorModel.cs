using System.Collections.Generic;
using Ks.Web.Framework.Mvc;

namespace Ks.Web.Models.Common
{
    public partial class LanguageSelectorModel : BaseKsModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public int CurrentLanguageId { get; set; }

        public bool UseImages { get; set; }
    }
}