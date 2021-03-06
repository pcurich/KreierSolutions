﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Batchs;
using Ks.Core.Domain.Reports;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Batchs
{
    [Validator(typeof(ScheduleBatchValidator))]
    public partial class ScheduleBatchModel : BaseKsEntityModel
    {
        public ScheduleBatchModel()
        {
            AvailableFrecuencies = new List<SelectListItem>();
            AvailableMonths = new List<SelectListItem>();
            AvailableYears = new List<SelectListItem>();
            ReportInfo = new ReportInfo();
            ReportInterfaceFile =  new ReportInterface();
            ReportInterfaceTable = new ReportInterface();
        }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        public string SystemName { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.PathBase")]
        [AllowHtml]
        public string PathBase { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.PathRead")]
        [AllowHtml]
        public string FolderRead { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.PathLog")]
        [AllowHtml]
        public string FolderLog { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.PathMoveToDone")]
        [AllowHtml]
        public string FolderMoveToDone { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.PathMoveToError")]
        [AllowHtml]
        public string FolderMoveToError { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.Frecuency")]
        [AllowHtml]
        public string FrecuencyName { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.Frecuency")]
        [AllowHtml]
        public int FrecuencyId { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.PeriodYear")]
        public int PeriodYear { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.PeriodMonth")]
        public int PeriodMonth { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.StartExecutionOn")]
        [AllowHtml]
        [UIHint("DateTimeNullable")]
        public DateTime? StartExecutionOn { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.NextExecutionOn")]
        [AllowHtml]
        [UIHint("DateTimeNullable")]
        public DateTime? NextExecutionOn { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.LastExecutionOn")]
        [AllowHtml]
        [UIHint("DateTimeNullable")]
        public DateTime? LastExecutionOn { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.Enabled")]
        [AllowHtml]
        public bool Enabled { get; set; }

        public bool UpdateData { get; set; }

        public List<SelectListItem> AvailableMonths { get; set; }
        public List<SelectListItem> AvailableYears { get; set; }
        public List<SelectListItem> AvailableFrecuencies { get; set; }

        public ReportInfo ReportInfo { get; set; }
        public ReportInterface ReportInterfaceTable { get; set; }
        public ReportInterface ReportInterfaceFile { get; set; }
        public List<ReportSummaryMerge> ReportSummaryMerges { get; set; }
    }

    public class ReportInfo
    {
        public ReportInfo()
        {
            Types = new List<SelectListItem>();
            SubTypes = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.Year")]
        public int YearId { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.Month")]
        public int MonthId { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.Type")]
        public int TypeId { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.SubType")]
        public int SubTypeId { get; set; }


        public List<SelectListItem> AvailableMonths { get; set; }
        public List<SelectListItem> AvailableYears { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> SubTypes { get; set; }


    }

    public class ReportInterface
    {
        public ReportInterface()
        {
            Types = new List<SelectListItem>();
            States = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.Year")]
        public int YearId { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.Month")]
        public int MonthId { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.Type")]
        public int TypeId { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Fields.ReportInfo.State")]
        public int StateId { get; set; }

        public List<SelectListItem> AvailableMonths { get; set; }
        public List<SelectListItem> AvailableYears { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> States { get; set; }
    }
}