using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Messages;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Messages
{
    [Validator(typeof(QueuedEmailValidator))]
    public partial class QueuedEmailModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.Id")]
        public override int Id { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.Priority")]
        public string PriorityName { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.From")]
        [AllowHtml]
        public string From { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.FromName")]
        [AllowHtml]
        public string FromName { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.To")]
        [AllowHtml]
        public string To { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.ToName")]
        [AllowHtml]
        public string ToName { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyTo")]
        [AllowHtml]
        public string ReplyTo { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyToName")]
        [AllowHtml]
        public string ReplyToName { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.CC")]
        [AllowHtml]
        public string Cc { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.Bcc")]
        [AllowHtml]
        public string Bcc { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.AttachmentFilePath")]
        [AllowHtml]
        public string AttachmentFilePath { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.SentTries")]
        public int SentTries { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.SentOn")]
        public DateTime? SentOn { get; set; }

        [KsResourceDisplayName("Admin.System.QueuedEmails.Fields.EmailAccountName")]
        [AllowHtml]
        public string EmailAccountName { get; set; }
    }
}