using System.Collections.Generic;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Messages
{
    public partial class MessageTemplateListModel : BaseKsModel
    {
        public MessageTemplateListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.SearchStore")]
        public int SearchStoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
    }
}