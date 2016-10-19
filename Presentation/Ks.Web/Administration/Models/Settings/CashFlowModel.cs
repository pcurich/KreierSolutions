using System;
using System.ComponentModel.DataAnnotations;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Settings
{
    [Serializable]
    public class CashFlowModel
    {
        public int Id { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.CashFlow.Since")]
        public decimal Since { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.CashFlow.To")]
        public decimal To { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.CashFlow.Amount")]
        [UIHint("Decimal")]
        public decimal Amount { get; set; }
    }
}