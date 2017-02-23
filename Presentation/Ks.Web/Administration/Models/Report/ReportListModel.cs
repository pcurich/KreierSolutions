using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Ks.Admin.Models.Report
{

    public class ReportListModel
    {
        public ReportGlobal ReportGlobal { get; set; }
        public ReportBenefit ReportBenefit { get; set; }
        public ReportLoan ReportLoan { get; set; }
        public ReportContribution ReportContribution { get; set; }
        public ReportMilitarySituation ReportMilitarySituation { get; set; }
        public SumaryBankPayment SumaryBankPayment  { get; set; }

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

    public class ReportContribution
    {
        public ReportContribution()
        {
            Types = new List<SelectListItem>();
            To = new List<SelectListItem>();
            From = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Catalog.ReportContribution.Fields.From")]
        public int FromId { get; set; }
        [KsResourceDisplayName("Admin.Catalog.ReportContribution.Fields.To")]
        public int ToId { get; set; }
        [KsResourceDisplayName("Admin.Catalog.ReportContribution.Fields.Type")]
        public int TypeId { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> To { get; set; }
        public List<SelectListItem> From { get; set; }
    }

    public class ReportBenefit
    {
        public ReportBenefit()
        {
            Types = new List<SelectListItem>();
            Sources = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Catalog.ReportBenefit.Fields.From")]
        [UIHint("DateNullable")]
        public DateTime? From { get; set; }
        [KsResourceDisplayName("Admin.Catalog.ReportBenefit.Fields.To")]
        [UIHint("DateNullable")]
        public DateTime? To { get; set; }
        [KsResourceDisplayName("Admin.Catalog.ReportBenefit.Fields.Type")]
        public int TypeId { get; set; }
        public List<SelectListItem> Types { get; set; }
        [KsResourceDisplayName("Admin.Catalog.ReportBenefit.Fields.Source")]
        public int SourceId { get; set; }
        public List<SelectListItem> Sources { get; set; }
    }

    public class ReportMilitarySituation
    {
        public ReportMilitarySituation()
        {
            MilitarySituations = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Catalog.ReportMilitarySituation.Fields.MilitarySituation")]
        public int MilitarySituationId { get; set; }
        public List<SelectListItem> MilitarySituations { get; set; }

        [KsResourceDisplayName("Admin.Catalog.ReportMilitarySituation.Fields.ContributionState")]
        public int ContributionStateId { get; set; }
        public List<SelectListItem> ContributionStates { get; set; }

        [KsResourceDisplayName("Admin.Catalog.ReportMilitarySituation.Fields.LoanState")]
        public int LoanStateId { get; set; }
        public List<SelectListItem> LoanStates { get; set; }
    }

    public class SumaryBankPayment
    {
        public SumaryBankPayment()
        {
            Sources= new List<SelectListItem>();
            Types = new List<SelectListItem>();
        }

        [UIHint("DateNullable")]
        [KsResourceDisplayName("Admin.Catalog.SumaryBankPayment.Fields.From")]
        public DateTime? From { get; set; }
        [UIHint("DateNullable")]
        [KsResourceDisplayName("Admin.Catalog.SumaryBankPayment.Fields.To")]
        public DateTime? To { get; set; }

        public List<SelectListItem> Sources { get; set; }
        public List<SelectListItem> Types { get; set; }
        [KsResourceDisplayName("Admin.Catalog.SumaryBankPayment.Fields.Type")]
        public int TypeId { get; set; }
        [KsResourceDisplayName("Admin.Catalog.SumaryBankPayment.Fields.Source")]
        public int SourceId { get; set; }
    }

    #endregion
}