﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;
using Ks.Web.Validators.Customer;

namespace Ks.Web.Models.Customer
{
    [Validator(typeof(PasswordRecoveryConfirmValidator))]
    public partial class PasswordRecoveryConfirmModel : BaseKsModel
    {
        [AllowHtml]
        [DataType(DataType.Password)]
        [KsResourceDisplayName("Account.PasswordRecovery.NewPassword")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [KsResourceDisplayName("Account.PasswordRecovery.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public bool DisablePasswordChanging { get; set; }
        public string Result { get; set; }
    }
}