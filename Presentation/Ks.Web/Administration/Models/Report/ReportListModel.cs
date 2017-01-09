using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Report
{

    public class ReportListModel
    {
        public ReportGlobal ReportGlobal { get; set; }
        public ReportCheck ReportCheck { get; set; }
        public ReportLoan ReportLoan { get; set; }

    }

    #region Inner Class

    public class ReportGlobal
    {
        public ReportGlobal()
        {
            Sources = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            Months = new List<SelectListItem>();
            Types = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Catalog.ReportGlobal.Fields.Source")]
        public int SourceId { get; set; }

        public List<SelectListItem> Sources { get; set; }

        [KsResourceDisplayName("Admin.Catalog.ReportGlobal.Fields.Type")]
        public int TypeId { get; set; }

        public List<SelectListItem> Types { get; set; }

        [KsResourceDisplayName("Admin.Catalog.ReportGlobal.Fields.Month")]
        public int Month { get; set; }

        public List<SelectListItem> Months { get; set; }

        [KsResourceDisplayName("Admin.Catalog.ReportGlobal.Fields.Year")]
        public int Year { get; set; }

        public List<SelectListItem> Years { get; set; }
    }

    public class ReportLoan
    {
        public ReportLoan()
        {
            Types = new List<SelectListItem>();
            States = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Catalog.ReportLoan.Fields.State")]
        public int StatesId { get; set; }
        public List<SelectListItem> States { get; set; }

        [KsResourceDisplayName("Admin.Catalog.ReportLoan.Fields.From")]
        [UIHint("DateNullable")]
        public DateTime? From { get; set; }
        [KsResourceDisplayName("Admin.Catalog.ReportLoan.Fields.To")]
        [UIHint("DateNullable")]
        public DateTime? To { get; set; }
        [KsResourceDisplayName("Admin.Catalog.ReportLoan.Fields.Type")]
        public int TypeId { get; set; }
        public List<SelectListItem> Types { get; set; }
    }

    public class ReportCheck
    {

    }

    #endregion
}