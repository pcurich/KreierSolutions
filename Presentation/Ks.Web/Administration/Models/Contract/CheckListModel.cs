using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Contract
{
    public class CheckListModel
    {
        public CheckListModel()
        {
            Entities= new List<SelectListItem>();
            Banks= new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Contract.Check.Fields.SearchFrom")]
        [UIHint("DateNullable")]
        public DateTime? SearchFrom { get; set; }

        [KsResourceDisplayName("Admin.Contract.Check.Fields.SearchTo")]
        [UIHint("DateNullable")]
        public DateTime? SearchTo { get; set; }

        [KsResourceDisplayName("Admin.Contract.Check.Fields.EntityName")]
        public string EntityName { get; set; }
        public List<SelectListItem> Entities { get; set; }

        [KsResourceDisplayName("Admin.Contract.Check.Fields.BankName")]
        public string BankName { get; set; }
        public List<SelectListItem> Banks { get; set; }

        [KsResourceDisplayName("Admin.Contract.Check.Fields.CheckNumber")]
        public string CheckNumber { get; set; }
        
    }
}