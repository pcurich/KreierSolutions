using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Models.Customers;
using Ks.Admin.Validators.Messages;
using Ks.Web.Framework;
using Ks.Web.Framework.Localization;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Messages
{
    [Validator(typeof(MessageTemplateValidator))]
    public partial class MessageTemplateModel : BaseKsEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
    {
        public MessageTemplateModel()
        {
            Locales = new List<MessageTemplateLocalizedModel>();
            AvailableEmailAccounts = new List<EmailAccountModel>();
            AvailableStores = new List<CustomerModel.StoreModel>();
        }


        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
        [AllowHtml]
        public bool IsActive { get; set; }

        public bool HasAttachedDownload { get; set; }
        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<EmailAccountModel> AvailableEmailAccounts { get; set; }

        //Store mapping
        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AvailableStores")]
        public List<CustomerModel.StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }
        //comma-separated list of stores used on the list page
        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public string ListOfStores { get; set; }



        public IList<MessageTemplateLocalizedModel> Locales { get; set; }
    }

    public partial class MessageTemplateLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [KsResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
    }
}