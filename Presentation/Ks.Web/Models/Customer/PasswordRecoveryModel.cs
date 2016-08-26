using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;
using Ks.Web.Validators.Customer;

namespace Ks.Web.Models.Customer
{
    [Validator(typeof(PasswordRecoveryValidator))]
    public partial class PasswordRecoveryModel : BaseKsModel
    {
        [AllowHtml]
        [KsResourceDisplayName("Account.PasswordRecovery.Email")]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}