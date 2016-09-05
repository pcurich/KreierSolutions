using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Messages;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Messages
{
    [Validator(typeof(EmailAccountValidator))]
    public partial class EmailAccountModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.DisplayName")]
        [AllowHtml]
        public string DisplayName { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Host")]
        [AllowHtml]
        public string Host { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Port")]
        public int Port { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.EnableSsl")]
        public bool EnableSsl { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }

        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.IsDefaultEmailAccount")]
        public bool IsDefaultEmailAccount { get; set; }


        [KsResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.SendTestEmailTo")]
        [AllowHtml]
        public string SendTestEmailTo { get; set; }

    }
}